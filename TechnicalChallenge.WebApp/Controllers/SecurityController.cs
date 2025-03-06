using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechnicalChallenge.BusinessService;
using TechnicalChallenge.WebApp.Models;

namespace TechnicalChallenge.WebApp.Controllers
{
    public class SecurityController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(ICustomerService customerService, ILogger<SecurityController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Logs out the current user and redirects to the login page.
        /// </summary>
        /// <returns>A redirect to the login page.</returns>
        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            _logger.LogInformation("User logging out");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out successfully");
            return RedirectToAction("Login", "Security");
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <returns>A view displaying the login form.</returns>
        [HttpGet]
        public async Task<IActionResult> LoginAsync()
        {
            _logger.LogInformation("Navigating to login page");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.HideLogout = true;
            return View();
        }

        /// <summary>
        /// Handles the login process for a user.
        /// </summary>
        /// <param name="model">The login view model containing the user's login details.</param>
        /// <returns>A redirect to the home page if login is successful, otherwise redisplays the login form with an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Attempting to log in user with name: {Name}", model.Name);
                var getByNameResult = await _customerService.GetByNameAsync(model.Name);

                if (getByNameResult.IsSuccess)
                {
                    _logger.LogInformation("User {Name} found, creating claims", model.Name);
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, model.Name),
                        new(ClaimTypes.NameIdentifier, getByNameResult.Value.CustomerNumber.ToString()),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                        IsPersistent = true,
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation("User {Name} logged in successfully", model.Name);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogWarning("Login attempt failed for user {Name}", model.Name);
                    ModelState.AddModelError(nameof(model.Name), "Not a valid name");
                }
            }
            else
            {
                _logger.LogWarning("Login attempt with invalid model state for user {Name}", model.Name);
            }

            return View(model);
        }
    }
}

