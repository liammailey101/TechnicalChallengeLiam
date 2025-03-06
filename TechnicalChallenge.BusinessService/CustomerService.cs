using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using TechnicalChallenge.BusinessService.DataTransferObjects;
using TechnicalChallenge.Common;
using TechnicalChallenge.Data.Domain;
using TechnicalChallenge.Data.Respository;

namespace TechnicalChallenge.BusinessService
{
    /// <summary>
    /// Service for handling customer-related operations.
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets a customer by their name.
        /// </summary>
        /// <param name="name">The name of the customer.</param>
        /// <returns>A result containing the customer details.</returns>
        /// <example>
        /// var result = await customerService.GetByNameAsync("John Doe");
        /// if (result.IsSuccess)
        /// {
        ///     var customer = result.Value;
        ///     // Use the customer details
        /// }
        /// else
        /// {
        ///     // Handle the error
        /// }
        /// </example>
        public async Task<Result<CustomerDto>> GetByNameAsync(string name)
        {
            try
            {
                _logger.LogInformation("Getting customer by name: {Name}", name);

                var repository = _unitOfWork.GetRepository<Customer>();

                var customer = await repository.FirstAsync(f => f.FirstName.Equals(name, StringComparison.CurrentCultureIgnoreCase));

                if (customer == null)
                {
                    _logger.LogWarning("Customer with name {Name} not found", name);
                    return Result.Failure<CustomerDto>(ResultError.RecordNotFound);
                }

                var customerItem = _mapper.Map<CustomerDto>(customer);
                _logger.LogInformation("Customer with name {Name} found", name);
                return Result<CustomerDto>.Success(customerItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting customer by name: {Name}", name);
                return Result.Failure<CustomerDto>(new ResultError("Error.GetByName", "An error occurred while getting customer by name"));
            }
        }

        /// <summary>
        /// Gets the accounts for a specific customer.
        /// </summary>
        /// <param name="customerNumber">The customer identifier.</param>
        /// <returns>A result containing a list of accounts for the specified customer.</returns>
        /// <example>
        /// var result = await customerService.GetAccountsAsync(customerNumber);
        /// if (result.IsSuccess)
        /// {
        ///     var accounts = result.Value;
        ///     // Use the accounts
        /// }
        /// else
        /// {
        ///     // Handle the error
        /// }
        /// </example>
        public async Task<Result<List<AccountDto>>> GetAccountsAsync(Guid customerNumber)
        {
            try
            {
                _logger.LogInformation("Getting accounts for customer number: {CustomerNumber}", customerNumber);

                var repository = _unitOfWork.GetRepository<Account>();

                var accounts = await repository.FindAsync(a => a.Customer.CustomerNumber == customerNumber, new Expression<Func<Account, object>>[] { c => c.AccountType });

                if (accounts == null)
                {
                    _logger.LogWarning("Accounts for customer number {CustomerNumber} not found", customerNumber);
                    return Result.Failure<List<AccountDto>>(ResultError.RecordNotFound);
                }

                var accountsDtoList = _mapper.Map<List<AccountDto>>(accounts);
                _logger.LogInformation("Accounts for customer number {CustomerNumber} found", customerNumber);
                return Result<List<AccountDto>>.Success(accountsDtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting accounts for customer number: {CustomerNumber}", customerNumber);
                return Result.Failure<List<AccountDto>>(new ResultError("Error.GetAccounts", "An error occurred while getting accounts for customer"));
            }
        }

        /// <summary>
        /// Transfers funds between two accounts.
        /// </summary>
        /// <param name="sourceAccountId">The source account identifier.</param>
        /// <param name="targetAccountId">The target account identifier.</param>
        /// <param name="amount">The amount to transfer.</param>
        /// <returns>A result indicating the success or failure of the operation, including the balances of the source and target accounts.</returns>
        /// <example>
        /// var result = await customerService.TransferFunds(sourceAccountId, targetAccountId, 100.00m);
        /// if (result.IsSuccess)
        /// {
        ///     var balances = result.Value;
        ///     // Use the balances
        /// }
        /// else
        /// {
        ///     // Handle the error
        /// }
        /// </example>
        public async Task<Result<(decimal SourceAccountBalance, decimal TargetAccountBalance)>> TransferFunds(Guid sourceAccountId, Guid targetAccountId, decimal amount)
        {
            try
            {
                _logger.LogInformation("Transferring {Amount} from account {SourceAccountId} to account {TargetAccountId}", amount, sourceAccountId, targetAccountId);

                var repository = _unitOfWork.GetRepository<Account>();

                var sourceAccount = await repository.FirstAsync(a => a.AccountId == sourceAccountId);
                var targetAccount = await repository.FirstAsync(a => a.AccountId == targetAccountId);

                if (sourceAccount == null || targetAccount == null)
                {
                    _logger.LogWarning("Source or target account not found for transfer");
                    return Result.Failure<(decimal, decimal)>(new ResultError("AccountNotFound", "Source or target account not found"));
                }

                sourceAccount.Balance -= amount;
                targetAccount.Balance += amount;

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Transferred {Amount} from account {SourceAccountId} to account {TargetAccountId}", amount, sourceAccountId, targetAccountId);
                return Result<(decimal SourceAccountBalance, decimal TargetAccountBalance)>.Success((sourceAccount.Balance, targetAccount.Balance));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while transferring {Amount} from account {SourceAccountId} to account {TargetAccountId}", amount, sourceAccountId, targetAccountId);
                return Result.Failure<(decimal, decimal)>(new ResultError("Error.TransferFunds", "An error occurred while transferring funds"));
            }
        }
    }
}