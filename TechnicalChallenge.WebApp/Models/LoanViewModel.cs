using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TechnicalChallenge.BusinessService.DataTransferObjects;

namespace TechnicalChallenge.WebApp.Models
{
    /// <summary>
    /// ViewModel for handling loan requests and displaying loan information.
    /// </summary>
    public class LoanViewModel
    {
        /// <summary>
        /// Gets or sets the list of accounts available for loan payment.
        /// </summary>
        /// <value>The list of accounts.</value>
        public List<AccountDto> Accounts { get; set; }

        /// <summary>
        /// Gets or sets the selected account identifier for loan payment.
        /// </summary>
        /// <value>The selected account identifier.</value>
        [DisplayName("Account for loan payment")]
        public Guid SelectedAccountId { get; set; }

        /// <summary>
        /// Gets or sets the selected loan duration in years.
        /// </summary>
        /// <value>The selected loan duration.</value>
        [DisplayName("Loan duration in years")]
        public byte SelectedDuration { get; set; }

        /// <summary>
        /// Gets or sets the loan amount.
        /// </summary>
        /// <value>The loan amount.</value>
        [Required(ErrorMessage = "Loan amount is required")]
        [DisplayName("Loan amount")]
        public decimal? LoanAmount { get; set; }

        /// <summary>
        /// Gets or sets the loan rate.
        /// </summary>
        /// <value>The loan rate.</value>
        public byte LoanRate { get; set; }

        /// <summary>
        /// Gets or sets the list of available loan durations.
        /// </summary>
        /// <value>The list of available loan durations.</value>
        public List<byte> LoanDurations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a loan has been requested.
        /// </summary>
        /// <value><c>true</c> if a loan has been requested; otherwise, <c>false</c>.</value>
        public bool IsLoanRequested { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether a loan has been approved.
        /// </summary>
        /// <value><c>true</c> if a loan has been approved; otherwise, <c>false</c>.</value>
        public bool IsLoanApproved { get; set; } = false;
    }
}