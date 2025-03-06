using TechnicalChallenge.BusinessService.DataTransferObjects;

namespace TechnicalChallenge.WebApp.Models
{
    /// <summary>
    /// ViewModel for displaying account and loan information on the index page.
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Gets or sets the list of accounts.
        /// </summary>
        /// <value>The list of accounts.</value>
        public required List<AccountDto> Accounts { get; set; }

        /// <summary>
        /// Gets or sets the list of loans.
        /// </summary>
        /// <value>The list of loans.</value>
        public required List<AccountDto> Loans { get; set; }
    }
}