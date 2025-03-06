using TechnicalChallenge.BusinessService.DataTransferObjects;
using TechnicalChallenge.Common;

namespace TechnicalChallenge.BusinessService
{
    /// <summary>
    /// Interface for customer-related operations.
    /// </summary>
    public interface ICustomerService
    {
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
        Task<Result<CustomerDto>> GetByNameAsync(string name);

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
        Task<Result<List<AccountDto>>> GetAccountsAsync(Guid customerNumber);

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
        Task<Result<(decimal SourceAccountBalance, decimal TargetAccountBalance)>> TransferFunds(Guid sourceAccountId, Guid targetAccountId, decimal amount);
    }
}
