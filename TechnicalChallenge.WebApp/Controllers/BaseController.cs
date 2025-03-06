using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TechnicalChallenge.WebApp.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Retrieves the customer number from the current user's claims.
        /// </summary>
        /// <returns>The customer number as a GUID.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the customer number is not a valid GUID.</exception>
        protected Guid GetCustomerNumber()
        {
            var customerNumberString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(customerNumberString, out var customerNumber))
            {
                return customerNumber;
            }
            else
            {
                throw new InvalidOperationException($"Invalid customer number GUID: {customerNumberString}");
            }
        }
    }
}