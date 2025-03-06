using AutoMapper;
using Microsoft.Extensions.Logging;
using TechnicalChallenge.BusinessService.DataTransferObjects;
using TechnicalChallenge.Common;
using TechnicalChallenge.Data.Domain;
using TechnicalChallenge.Data.Respository;

namespace TechnicalChallenge.BusinessService
{
    /// <summary>
    /// Service for handling loan-related operations.
    /// </summary>
    public class LoanService : ILoanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoanService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public LoanService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LoanService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the available loan durations.
        /// </summary>
        /// <returns>A result containing a list of unique loan durations.</returns>
        /// <example>
        /// var result = await loanService.GetLoanDurations();
        /// if (result.IsSuccess)
        /// {
        ///     var durations = result.Value;
        ///     // Use the durations
        /// }
        /// else
        /// {
        ///     // Handle the error
        /// }
        /// </example>
        public async Task<Result<List<byte>>> GetLoanDurations()
        {
            try
            {
                _logger.LogInformation("Getting loan durations");

                var repository = _unitOfWork.GetRepository<LoanRate>();

                var rates = await repository.GetAllAsync();

                if (rates == null)
                {
                    _logger.LogWarning("No loan rates found");
                    return Result.Failure<List<byte>>(ResultError.RecordNotFound);
                }

                var uniqueDurations = rates.Select(r => r.Duration).Distinct().ToList();
                _logger.LogInformation("Loan durations retrieved successfully");
                return Result<List<byte>>.Success(uniqueDurations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting loan durations");
                return Result.Failure<List<byte>>(new ResultError("Error.GetLoanDurations", "An error occurred while getting loan durations"));
            }
        }

        /// <summary>
        /// Gets the loan rate for a specific customer and duration.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="duration">The loan duration.</param>
        /// <returns>A result containing the loan rate for the specified customer and duration.</returns>
        /// <example>
        /// var result = await loanService.GetLoanRate(customerId, duration);
        /// if (result.IsSuccess)
        /// {
        ///     var loanRate = result.Value;
        ///     // Use the loan rate
        /// }
        /// else
        /// {
        ///     // Handle the error
        /// }
        /// </example>
        public async Task<Result<LoanRateDto>> GetLoanRate(Guid customerId, int duration)
        {
            try
            {
                _logger.LogInformation("Getting loan rate for customer {CustomerId} and duration {Duration}", customerId, duration);

                var customerRepo = _unitOfWork.GetRepository<Customer>();

                var customer = await customerRepo.FirstAsync(c => c.CustomerNumber == customerId);

                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", customerId);
                    return Result.Failure<LoanRateDto>(ResultError.RecordNotFound);
                }

                var loanRepository = _unitOfWork.GetRepository<LoanRate>();

                var rate = await loanRepository.FirstAsync(r => r.Duration == duration &&
                    (r.RatingFrom <= customer.CreditScore && r.RatingTo > customer.CreditScore));

                if (rate == null)
                {
                    _logger.LogWarning("Loan rate not found for customer {CustomerId} with duration {Duration}", customerId, duration);
                    return Result.Failure<LoanRateDto>(ResultError.RecordNotFound);
                }

                var rateDto = _mapper.Map<LoanRateDto>(rate);
                _logger.LogInformation("Loan rate retrieved successfully for customer {CustomerId} with duration {Duration}", customerId, duration);
                return Result<LoanRateDto>.Success(rateDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting loan rate for customer {CustomerId} and duration {Duration}", customerId, duration);
                return Result.Failure<LoanRateDto>(new ResultError("Error.GetLoanRate", "An error occurred while getting loan rate"));
            }
        }

        /// <summary>
        /// Processes a loan for a customer.
        /// </summary>
        /// <param name="amount">The loan amount.</param>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="duration">The loan duration.</param>
        /// <param name="targetAccount">The target account identifier.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        /// <example>
        /// var result = await loanService.ProcessLoan(amount, customerId, duration, targetAccount);
        /// if (result.IsSuccess)
        /// {
        ///     // Loan processed successfully
        /// }
        /// else
        /// {
        ///     // Handle the error
        /// }
        /// </example>
        public async Task<Result> ProcessLoan(decimal amount, Guid customerId, int duration, Guid targetAccount)
        {
            try
            {
                _logger.LogInformation("Processing loan for customer {CustomerId} with amount {Amount} and duration {Duration}", customerId, amount, duration);

                var loanRateRepo = _unitOfWork.GetRepository<LoanRate>();
                var customerRepo = _unitOfWork.GetRepository<Customer>();
                var accountRepo = _unitOfWork.GetRepository<Account>();

                var customer = await customerRepo.FirstAsync(c => c.CustomerNumber == customerId);
                var account = await accountRepo.FirstAsync(a => a.AccountId == targetAccount);

                if (customer == null || account == null || account.CustomerId != customer.Id)
                {
                    _logger.LogWarning("Invalid account for customer {CustomerId}", customerId);
                    return Result.Failure(new ResultError("InvalidAccount", "Customer does not have this account"));
                }

                var rate = await loanRateRepo.FirstAsync(r => r.Duration == duration &&
                    (r.RatingFrom <= customer.CreditScore && r.RatingTo > customer.CreditScore));

                if (rate == null)
                {
                    _logger.LogWarning("Customer {CustomerId} does not have adequate credit rating for duration {Duration}", customerId, duration);
                    return Result.Failure(new ResultError("BadRating", "Customer does not have adequate credit rating"));
                }

                var loanAccount = new Account
                {
                    Balance = amount,
                    AccountNumber = GenerateAccountNumber(),
                    CustomerId = customer.Id,
                    AccountTypeId = (int)AccountType.Loan,
                    CreatedBy = "System",
                    CreatedDate = DateTime.Now
                };

                await accountRepo.AddAsync(loanAccount);
                account.Balance += amount;
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Loan processed successfully for customer {CustomerId} with amount {Amount} and duration {Duration}", customerId, amount, duration);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing loan for customer {CustomerId} with amount {Amount} and duration {Duration}", customerId, amount, duration);
                return Result.Failure(new ResultError("Error.ProcessLoan", "An error occurred while processing loan"));
            }
        }

        /// <summary>
        /// Generates a random account number.
        /// </summary>
        /// <returns>A randomly generated account number.</returns>
        /// <example>
        /// var accountNumber = GenerateAccountNumber();
        /// // Use the account number
        /// </example>
        private string GenerateAccountNumber()
        {
            _logger.LogInformation("Generating account number");

            // This is purely for a quick demo, in a real world scenario this would be more complex and take into account existing account numbers
            var random = new Random();
            var accountNumber = new char[8];
            for (int i = 0; i < accountNumber.Length; i++)
            {
                accountNumber[i] = (char)('0' + random.Next(0, 10));
            }

            var generatedAccountNumber = new string(accountNumber);
            _logger.LogInformation("Generated account number: {AccountNumber}", generatedAccountNumber);
            return generatedAccountNumber;
        }
    }
}