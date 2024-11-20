using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Learning_Course.ViewModels;
using E_Learning_Course.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using E_Learning_Course.Service;
using System.Security.Claims;

namespace E_Learning_Course.WebApp.Pages.Homepage
{
    public class EditProfileModel : PageModel
    {
        private readonly IFileService _fileService;
        private readonly UserManager<User> _userManager;
        private readonly IProfileEditService _profileEditService;
        [BindProperty]
        public UserEditDTO userEdit { get; set; }
        public string msgProfile { get; set; }
        public string msgChangePass { get; set; }

        public string role { get; set; }
        public User UPlaceholder;
        [BindProperty]
        public IFormFile fileAvatar { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống")]
        public string? currentPassword { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới để thay đổi")]

        [StringLength(100, ErrorMessage = "Mật khẩu mới phải chứa ít nhất {2} ký tự.", MinimumLength = 10)]
        public string? newPassword { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Xác nhận lại mật khẩu không được để trống")]
        //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? confirmPassword { get; set; }
        public EditProfileModel(UserManager<User> userManager, IFileService fileService, IProfileEditService profileEditService)
        {
            _userManager = userManager;
            _fileService = fileService;
            _profileEditService = profileEditService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var name = User.Identity.Name;
            if (name == null)
            {
                return RedirectToPage("../Error");
            }
            var user = await _userManager.FindByNameAsync(name);
            UPlaceholder = user;
            IList<string> roles = await _userManager.GetRolesAsync(user);
            role = roles.FirstOrDefault().ToString();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var name = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(name);
                UPlaceholder = user;
                IList<string> roles = await _userManager.GetRolesAsync(user);
                role = roles.FirstOrDefault().ToString();
                ModelState.Remove("currentPassword");
                ModelState.Remove("newPassword");
                ModelState.Remove("confirmPassword");

                if (fileAvatar == null)
                {
                    ModelState.Remove("fileAvatar");
                }
                if (user != null && ModelState.IsValid)
                {
                    if (await _profileEditService.EditProfile(_userManager, _fileService, fileAvatar, userEdit, user))
                        msgProfile = "Lưu thay đổi thành công";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Page();
        }

        public async Task<IActionResult> OnPostPasswordAsync()
        {
            try
            {
                var name = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(name);
                UPlaceholder = user;
                ModelState.Remove("FirstName");
                ModelState.Remove("PhoneNumber");
                if (user == null)
                {
                    return RedirectToPage("../Error");
                }
                if (fileAvatar == null)
                {
                    ModelState.Remove("fileAvatar");
                }
                if (ModelState.IsValid)
                {
                    var checkPass = await _userManager.CheckPasswordAsync(user, currentPassword);
                    if (checkPass)
                    {
                        if (newPassword.Equals(confirmPassword))
                        {
                            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                            if (result.Succeeded)
                            {
                                msgChangePass = "Đổi mật khẩu thành công";
                                currentPassword = null;
                                newPassword = null;
                                confirmPassword = null;
                                ViewData["ShowPasswordForm"] = true;
                                return Page();
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("confirmPassword", "Xác nhận mật khẩu không đúng");
                            ViewData["ShowPasswordForm"] = true;
                            return Page();
                        }
                    }
                    ModelState.AddModelError("currentPassword", "Mật khẩu hiện tại không đúng");
                    ViewData["ShowPasswordForm"] = true;
                    return Page();
                }
                ViewData["ShowPasswordForm"] = true;
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ModelState.AddModelError(string.Empty, "An error occurred while changing the password.");
            }

            return Page();
        }

    }
}
