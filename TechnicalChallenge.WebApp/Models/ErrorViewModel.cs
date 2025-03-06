namespace TechnicalChallenge.WebApp.Models
{
    /// <summary>
    /// ViewModel for displaying error information.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>The request identifier.</value>
        public string? RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the request identifier should be shown.
        /// </summary>
        /// <value><c>true</c> if the request identifier should be shown; otherwise, <c>false</c>.</value>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}