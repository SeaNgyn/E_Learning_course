using E_Learning_Course.ViewModels;
using E_Learning_Course.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace E_Learning_Course.WebApp.Pages.Authentication
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IAuthenticateService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        [BindProperty(SupportsGet = true)] 
        public string? ReturnUrl { get; set; }

        public LoginModel(IAuthenticateService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public UserForAuthentication? Login { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Clear any existing error message when the page is first loaded
            ErrorMessage = null;
        }


        public async Task<IActionResult> OnPostAsync()
        {
            // Ensure the model is valid before processing
            if (!ModelState.IsValid)
                return Page();

            // Validate the user credentials using the authentication service
            var isValid = await _authService.ValidateUser(Login);
            if (!isValid)
            {
                // Set an error message to be displayed to the user if the authentication fails
                ErrorMessage = "Invalid email or password.";
                return Page();
            }
            var isConfirmed = await _authService.IsEmailConfirmed(Login.Email);
            if (!isConfirmed)
            {
                // Set an error message to be displayed to the user if the authentication fails
                ErrorMessage = "Please confirm your email before logging in.";
                return Page();
            }

            // Create a token if the user is valid
            var token = await _authService.CreateToken();

            // Store the token securely in an HTTP-only cookie
            _httpContextAccessor.HttpContext.Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = true, // Prevent client-side JavaScript access
                Secure = true,   // Ensure cookies are sent over HTTPS
                SameSite = SameSiteMode.Strict, // Protect against CSRF attacks
                Expires = DateTime.UtcNow.AddMinutes(30) // Token expiration
            });
            var roles = await _authService.GetUserRoles(Login.Email);

            if (roles.Contains("Administrator"))
            {
                return RedirectToPage("../Admin/Dashboard");
            }
            if (roles.Contains("Instructor"))
            {
                return RedirectToPage("../Admin/Courses/List");
            }
            else if (roles.Contains("Customer"))
            {
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }

                return RedirectToPage("../Homepage/Index");
            }


            // Redirect the user to the home page upon successful login
            return Page();
        }
    }
}
