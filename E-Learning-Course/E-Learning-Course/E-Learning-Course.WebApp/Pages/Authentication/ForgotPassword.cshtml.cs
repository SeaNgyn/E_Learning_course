using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.Utilities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;

namespace E_Learning_Course.WebApp.Pages.Authentication
{
    public static class PasswordGenerator
    {
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-";
            var random = new Random();
            char[] password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(password);
        }
    }

    public class ForgotPasswordModel : PageModel
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly ISendEmail _emailSender; // Inject the email service
        private readonly UserManager<User> _userManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly RepositoryContext _repositoryContext;
        [TempData]
        public string? ErrorMessage { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }
        public ForgotPasswordModel(IAuthenticateService authenticateService, ISendEmail emailSender, UserManager<User> userManager, UrlEncoder urlEncoder,RepositoryContext context)
        {
            _repositoryContext = context;
            _authenticateService = authenticateService;
            _emailSender = emailSender;
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Redisplay the form if validation fails
            }

            //var existingUserByUsername = await _userManager.FindByEmailAsync(UserAuthen.Email);
            //if (existingUserByUsername == null)
            //{
            //    ModelState.AddModelError(nameof(UserAuthen.Email), "Email is not Exist.");
            //    return Page();
            //}

            // Check if the email already exists
            var existingUserByEmail = await _userManager.FindByEmailAsync(Email);
            if (existingUserByEmail == null)
            {
                ModelState.AddModelError(nameof(Email), "Email does not existed");
                ErrorMessage = "Email does not existed";
                return Page();
            }
            //existingUserByEmail.EmailConfirmed = false;
            //_repositoryContext.SaveChanges();
            var loginToken = await _userManager.GenerateEmailConfirmationTokenAsync(existingUserByEmail);
            var loginLink = Url.PageLink(pageName: "/Authentication/Login", values: new { userId = existingUserByEmail.Id, token = loginToken });
            var newPassword = PasswordGenerator.GenerateRandomPassword();

            // Đặt lại mật khẩu mới cho người dùng
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUserByEmail);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(existingUserByEmail, resetToken, newPassword);

            if (!resetPasswordResult.Succeeded)
            {
                foreach (var error in resetPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page(); // Nếu việc đặt lại mật khẩu thất bại, hiển thị lại trang với lỗi
            }

            var emailContent = $@"
            <p>Hello {existingUserByEmail.UserName},</p>
            <p>We have reset your password. Your new password is: <strong>{newPassword}</strong></p>
            <p>You can <a href='{loginLink}'>click here</a> to login again with new password.</p>
                ";

            await _emailSender.SendEmailAsync(existingUserByEmail.Email, "Password Reset and Auto Login", emailContent);

            // Thông báo người dùng rằng email đã được gửi
            return RedirectToPage("ConfirmationEmailSent");
        }
    }
}


