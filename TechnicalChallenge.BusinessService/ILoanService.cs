using TechnicalChallenge.BusinessService.DataTransferObjects;
using TechnicalChallenge.Common;

namespace TechnicalChallenge.BusinessService
{
    /// <summary>
    /// Interface for loan-related operations.
    /// </summary>
    public interface ILoanService
    {
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
        Task<Result<List<byte>>> GetLoanDurations();

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
        Task<Result<LoanRateDto>> GetLoanRate(Guid customerId, int duration);

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
        Task<Result> ProcessLoan(decimal amount, Guid customerId, int duration, Guid targetAccount);
    }
}