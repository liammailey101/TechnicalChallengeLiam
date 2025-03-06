using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnicalChallenge.BusinessService;
using TechnicalChallenge.WebApp.Models;

namespace TechnicalChallenge.WebApp.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ICustomerService _customerService;
        private readonly ILoanService _loanService;

        public AccountController(ICustomerService customerService, ILoanService loanService, ILogger<AccountController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves the account details for a specific account.
        /// </summary>
        /// <param name="id">The unique identifier of the account.</param>
        /// <returns>A view displaying the account details.</returns>
        [HttpGet]
        [Route("Account/AccountDetail/{id}")]
        public async Task<IActionResult> AccountDetail(Guid id)
        {
            _logger.LogInformation("Retrieving account details for account ID: {AccountId}", id);
            var model = await GetAccountDetailAsync(id);
            if (model == null)
            {
                _logger.LogWarning("Account details not found for account ID: {AccountId}", id);
                return NotFound();
            }
            return View(model);
        }

        /// <summary>
        /// Handles the money transfer between accounts.
        /// </summary>
        /// <param name="model">The account detail view model containing transfer details.</param>
        /// <param name="id">The unique identifier of the account.</param>
        /// <returns>A view displaying the updated account details or an error message if the transfer fails.</returns>
        [HttpPost]
        [Route("Account/AccountDetail/{id}")]
        public async Task<IActionResult> AccountDetail(AccountDetailViewModel model, Guid id)
        {
            _logger.LogInformation("Handling money transfer for account ID: {AccountId}", id);
            var customerNumber = GetCustomerNumber();
            var getAccountsResult = await _customerService.GetAccountsAsync(customerNumber);
            if (getAccountsResult.IsSuccess)
            {
                var sourceAccount = getAccountsResult.Value.FirstOrDefault(a => a.AccountId == model.MoneyTransfer.SourceAccount);
                var targetAccount = getAccountsResult.Value.FirstOrDefault(a => a.AccountId == model.MoneyTransfer.TargetAccount);

                if (sourceAccount == null || targetAccount == null)
                {
                    _logger.LogWarning("Source or target account not found for transfer");
                    ModelState.AddModelError("", "Unable to transfer funds");
                }
                if (sourceAccount?.Balance < model.MoneyTransfer.Amount)
                {
                    _logger.LogWarning("Insufficient funds in source account for transfer");
                    ModelState.AddModelError("", "Source account does not have enough funds");
                }
                var transferResult = await _customerService.TransferFunds(model.MoneyTransfer.SourceAccount, model.MoneyTransfer.TargetAccount, model.MoneyTransfer.Amount.GetValueOrDefault());

                if (transferResult.IsSuccess)
                {
                    _logger.LogInformation("Funds transferred successfully from account ID: {SourceAccountId} to account ID: {TargetAccountId}", model.MoneyTransfer.SourceAccount, model.MoneyTransfer.TargetAccount);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("Failed to transfer funds from account ID: {SourceAccountId} to account ID: {TargetAccountId}", model.MoneyTransfer.SourceAccount, model.MoneyTransfer.TargetAccount);
                    ModelState.AddModelError("", "Unable to transfer funds");
                }
            }
            else
            {
                _logger.LogWarning("Failed to retrieve accounts for customer number: {CustomerNumber}", customerNumber);
            }
            model = await GetAccountDetailAsync(id);
            return View(model);
        }

        /// <summary>
        /// Retrieves the loan application view.
        /// </summary>
        /// <returns>A view displaying the loan application form.</returns>
        [HttpGet]
        [Route("Account/Loan")]
        public async Task<IActionResult> Loan()
        {
            _logger.LogInformation("Retrieving loan application view");
            var model = await GetLoanModel();
            return View(model);
        }

        /// <summary>
        /// Handles the loan application process.
        /// </summary>
        /// <param name="model">The loan view model containing loan application details.</param>
        /// <returns>A view displaying the loan application result or an error message if the application fails.</returns>
        [HttpPost]
        [Route("Account/Loan")]
        public async Task<IActionResult> Loan(LoanViewModel model)
        {
            _logger.LogInformation("Handling loan application for customer");
            var customerNumber = GetCustomerNumber();

            if (model.IsLoanApproved)
            {
                var processLoanResult = await _loanService.ProcessLoan(model.LoanAmount.GetValueOrDefault(), customerNumber, model.SelectedDuration, model.SelectedAccountId);
                if (processLoanResult.IsSuccess)
                {
                    _logger.LogInformation("Loan processed successfully for customer number: {CustomerNumber}", customerNumber);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("Failed to process loan for customer number: {CustomerNumber}", customerNumber);
                }
            }

            var getLoanRate = await _loanService.GetLoanRate(customerNumber, model.SelectedDuration);

            if (getLoanRate.IsSuccess)
            {
                model.LoanRate = getLoanRate.Value.Rate;
                model.IsLoanRequested = true;
                model.IsLoanApproved = true;
                _logger.LogInformation("Loan rate retrieved successfully for customer number: {CustomerNumber}", customerNumber);
            }
            else
            {
                model.IsLoanRequested = true;
                model.IsLoanApproved = false;
                _logger.LogWarning("Failed to retrieve loan rate for customer number: {CustomerNumber}", customerNumber);
            }

            return View(model);
        }

        /// <summary>
        /// Retrieves the loan model containing available accounts and loan durations.
        /// </summary>
        /// <returns>A loan view model with available accounts and loan durations.</returns>
        private async Task<LoanViewModel?> GetLoanModel()
        {
            _logger.LogInformation("Retrieving loan model");
            var customerNumber = GetCustomerNumber();

            var getAccountsResult = await _customerService.GetAccountsAsync(customerNumber);

            if (!getAccountsResult.IsSuccess || getAccountsResult.Value.Count == 0)
            {
                _logger.LogWarning("No accounts found for customer number: {CustomerNumber}", customerNumber);
                return null;
            }

            var nonLoneAccounts = getAccountsResult.Value.Where(a => a.AccountTypeId != (int)AccountType.Loan).ToList();
            var durationsResult = await _loanService.GetLoanDurations();

            var model = new LoanViewModel();
            model.Accounts = nonLoneAccounts;
            model.LoanDurations = durationsResult.Value.OrderBy(d => d).ToList();
            _logger.LogInformation("Loan model retrieved successfully for customer number: {CustomerNumber}", customerNumber);
            return model;
        }

        /// <summary>
        /// Retrieves the account detail view model for a specific account.
        /// </summary>
        /// <param name="id">The unique identifier of the account.</param>
        /// <returns>An account detail view model with account details and available transfer accounts.</returns>
        private async Task<AccountDetailViewModel> GetAccountDetailAsync(Guid id)
        {
            _logger.LogInformation("Retrieving account detail view model for account ID: {AccountId}", id);
            var customerNumber = GetCustomerNumber();
            var getAccountsResult = await _customerService.GetAccountsAsync(customerNumber);

            if (!getAccountsResult.IsSuccess || getAccountsResult.Value.Count == 0)
            {
                _logger.LogWarning("No accounts found for customer number: {CustomerNumber}", customerNumber);
                return null;
            }

            var selectedAccount = getAccountsResult.Value.FirstOrDefault(a => a.AccountId == id);
            if (selectedAccount == null)
            {
                _logger.LogWarning("Account not found for account ID: {AccountId}", id);
                return null;
            }
            var transferAccounts = getAccountsResult.Value.Where(a => a.AccountId != id && a.AccountTypeId != (int)AccountType.Loan).ToList();

            var model = new AccountDetailViewModel
            {
                Account = getAccountsResult.Value.First(a => a.AccountId == id),
                MaxTransferAmount = selectedAccount.Balance,
                AvailableAccounts = transferAccounts,
                MoneyTransfer = new MoneyTransferViewModel() { SourceAccount = id }
            };
            _logger.LogInformation("Account detail view model retrieved successfully for account ID: {AccountId}", id);
            return model;
        }
    }
}

