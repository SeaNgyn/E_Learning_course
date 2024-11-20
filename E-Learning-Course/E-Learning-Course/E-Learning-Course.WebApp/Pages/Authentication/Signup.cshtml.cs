using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Learning_Course.ViewModels;
using E_Learning_Course.Service;
using E_Learning_Course.Utilities;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using E_Learning_Course.Data.Entities;

namespace E_Learning_Course.WebApp.Pages.Authentication
{
    public class SignupModel : PageModel
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly ISendEmail _emailSender; // Inject the email service
        private readonly UserManager<User> _userManager;
        private readonly UrlEncoder _urlEncoder;

        [BindProperty]
        public UserForRegistration? User { get; set; }

        public SignupModel(IAuthenticateService authenticateService, ISendEmail emailSender, UserManager<User> userManager, UrlEncoder urlEncoder)
        {
            _authenticateService = authenticateService;
            _emailSender = emailSender;
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        // Handles form submission (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Redisplay the form if validation fails
            }

            // Check if the username already exists
            var existingUserByUsername = await _userManager.FindByNameAsync(User.UserName);
            if (existingUserByUsername != null)
            {
                ModelState.AddModelError("User.UserName", "Tên tài khoản đã tồn tại");
                return Page();
            }

            // Check if the email already exists
            var existingUserByEmail = await _userManager.FindByEmailAsync(User.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("User.Email", "Email này đã có người đăng kí");
                return Page();
            }

            // Register user using the authentication service
            var result = await _authenticateService.RegisterUser(User);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(User.UserName);
                if (user != null)
                {
                    // Generate the email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Create a confirmation link to be sent via email
                    var confirmationLink = Url.PageLink(pageName: "/Authentication/ConfirmEmail", values: new { userId = user.Id, token });

                    // Send confirmation email
                    await _emailSender.SendEmailAsync(User.Email, "Confirm your email", $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>");

                    // Redirect user to a page that informs them that the confirmation email has been sent
                    return RedirectToPage("ConfirmationEmailSent");
                }

                return RedirectToPage("Index"); // Redirect to a success page
            }

            // If registration failed, add errors to the ModelState and redisplay form
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
