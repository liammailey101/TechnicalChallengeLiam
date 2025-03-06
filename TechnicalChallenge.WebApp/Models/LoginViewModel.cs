using System.ComponentModel.DataAnnotations;

namespace TechnicalChallenge.WebApp.Models
{
    /// <summary>
    /// ViewModel for handling user login.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
    }
}