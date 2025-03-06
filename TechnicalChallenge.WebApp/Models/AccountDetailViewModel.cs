using System.ComponentModel.DataAnnotations;
using TechnicalChallenge.BusinessService.DataTransferObjects;

namespace TechnicalChallenge.WebApp.Models
{
    /// <summary>
    /// ViewModel for displaying account details and performing money transfers.
    /// </summary>
    public class AccountDetailViewModel
    {
        /// <summary>
        /// Gets or sets the account details.
        /// </summary>
        /// <value>The account details.</value>
        public required AccountDto Account { get; set; }

        /// <summary>
        /// Gets or sets the list of available accounts for money transfer.
        /// </summary>
        /// <value>The list of available accounts.</value>
        public required List<AccountDto> AvailableAccounts { get; set; }

        /// <summary>
        /// Gets or sets the maximum transfer amount.
        /// </summary>
        /// <value>The maximum transfer amount.</value>
        public required decimal MaxTransferAmount { get; set; }

        /// <summary>
        /// Gets or sets the money transfer details.
        /// </summary>
        /// <value>The money transfer details.</value>
        public MoneyTransferViewModel MoneyTransfer { get; set; }
    }

    /// <summary>
    /// ViewModel for performing money transfers between accounts.
    /// </summary>
    public class MoneyTransferViewModel
    {
        /// <summary>
        /// Gets or sets the source account identifier.
        /// </summary>
        /// <value>The source account identifier.</value>
        public Guid SourceAccount { get; set; }

        /// <summary>
        /// Gets or sets the target account identifier.
        /// </summary>
        /// <value>The target account identifier.</value>
        public Guid TargetAccount { get; set; }

        /// <summary>
        /// Gets or sets the transfer amount.
        /// </summary>
        /// <value>The transfer amount.</value>
        [Required(ErrorMessage = "Amount is required")]
        public decimal? Amount { get; set; }
    }
}