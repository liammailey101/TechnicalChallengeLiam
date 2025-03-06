using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechnicalChallenge.BusinessService;
using TechnicalChallenge.WebApp.Models;

namespace TechnicalChallenge.WebApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerService _customerService;

        public HomeController(ICustomerService customerService, ILogger<HomeController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Displays the home page with the customer's accounts and loans.
        /// </summary>
        /// <returns>A view displaying the customer's accounts and loans.</returns>
        public async Task<IActionResult> Index()
        {
            var customerNumber = GetCustomerNumber();
            _logger.LogInformation("Retrieving accounts for customer number: {CustomerNumber}", customerNumber);

            var getAccountsResult = await _customerService.GetAccountsAsync(customerNumber);

            IndexViewModel model = new() { Accounts = [], Loans = [] };

            if (getAccountsResult.IsSuccess)
            {
                model = new IndexViewModel
                {
                    Accounts = getAccountsResult.Value.Where(a => a.AccountTypeId != (int)AccountType.Loan).ToList(),
                    Loans = getAccountsResult.Value.Where(a => a.AccountTypeId == (int)AccountType.Loan).ToList()
                };
                _logger.LogInformation("Accounts retrieved successfully for customer number: {CustomerNumber}", customerNumber);
            }
            else
            {
                _logger.LogError("Failed to find accounts for customer: {CustomerNumber}", customerNumber);
            }
            return View(model);
        }

        /// <summary>
        /// Displays the error page.
        /// </summary>
        /// <returns>A view displaying the error information.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError("An error occurred. Request ID: {RequestId}", requestId);
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
