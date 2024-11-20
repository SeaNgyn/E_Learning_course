using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.Utilities;
using E_Learning_Course.ViewModels;
using Microsoft.EntityFrameworkCore; // Thêm dòng này
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Encodings.Web;

namespace E_Learning_Course.WebApp.Pages.Admin.Users
{
    public class CreateModel : PageModel
    {

        private readonly IStaffService _staffService;
        private readonly ISendEmail _emailSender; // Inject the email service
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UrlEncoder _urlEncoder;

        public string CurrentFilter { get; set; }
        [BindProperty]
        public string? Mess { get; set; }

        [BindProperty]
        public StaffForCreation? User { get; set; }

        public CreateModel(IStaffService staffService, ISendEmail emailSender, UserManager<User> userManager, UrlEncoder urlEncoder, RoleManager<IdentityRole> roleManager)
        {
            _staffService = staffService;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            Roles = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            return Page();
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
                ModelState.AddModelError(nameof(User.UserName), "Username is already taken.");
                return Page();
            }

            // Check if the email already exists
            var existingUserByEmail = await _userManager.FindByEmailAsync(User.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(nameof(User.Email), "An account with this email already exists.");
                return Page();
            }
            // Lấy ClaimsPrincipal của người dùng đang đăng nhập
            var currentUser = HttpContext.User; // User ở đây là ClaimsPrincipal của người đang đăng nhập   
            var randomPassword = GenerateRandomPassword();

            var result = await _staffService.AddStaff(User, currentUser, randomPassword);

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
                    await _emailSender.SendEmailAsync(User.Email, "Account is created", $"Here is your new password {randomPassword}");

                    Mess = "Add successful";
                    return Page();
                }

            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }


        private string GenerateRandomPassword(int length = 10)
        {
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digitChars = "0123456789";
            const string nonAlphanumericChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            // Đảm bảo rằng mật khẩu có ít nhất một ký tự từ mỗi loại
            var password = new StringBuilder();
            password.Append(lowercaseChars[new Random().Next(lowercaseChars.Length)]);
            password.Append(uppercaseChars[new Random().Next(uppercaseChars.Length)]);
            password.Append(digitChars[new Random().Next(digitChars.Length)]);
            password.Append(nonAlphanumericChars[new Random().Next(nonAlphanumericChars.Length)]);

            // Tạo phần còn lại của mật khẩu ngẫu nhiên
            var allChars = lowercaseChars + uppercaseChars + digitChars + nonAlphanumericChars;
            for (int i = 4; i < length; i++)
            {
                password.Append(allChars[new Random().Next(allChars.Length)]);
            }

            // Trộn lại mật khẩu để ngẫu nhiên hơn
            var randomPassword = new string(password.ToString().OrderBy(c => Guid.NewGuid()).ToArray());
            return randomPassword;
        }
    }
}
