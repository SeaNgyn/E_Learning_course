using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.Utilities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text.Encodings.Web;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace E_Learning_Course.WebApp.Pages.Admin.Users
{
    public class UpdateModel : PageModel
    {
        private readonly IStaffService _staffService;
        private readonly ISendEmail _emailSender; // Inject the email service
        private readonly UserManager<User> _userManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly RoleManager<IdentityRole> _roleManager; // Thêm RoleManager

        public string CurrentFilter { get; set; }

        [BindProperty]
        public StaffForCreation? User { get; set; }

        [BindProperty]
        public string? AvatarUrl { get; set; }

        [BindProperty]
        public string? Mess { get; set; }

        [BindProperty]
        public IList<string> Roles { get; set; } // Thêm thuộc tính Roles


        public UpdateModel(IStaffService staffService, ISendEmail emailSender, UserManager<User> userManager, UrlEncoder urlEncoder, RoleManager<IdentityRole> roleManager)
        {
            _staffService = staffService;
            _emailSender = emailSender;
            _userManager = userManager;
            _urlEncoder = urlEncoder;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin người dùng
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy tất cả các vai trò từ RoleManager
            var roleNames = _roleManager.Roles.Select(role => role.Name).ToList();

            // Lấy vai trò của người dùng
            var userRoles = await _userManager.GetRolesAsync(user);

            // Giả sử người dùng chỉ có 1 vai trò duy nhất
            var selectedRole = userRoles.FirstOrDefault();

            // Gán vai trò hiện tại và danh sách các vai trò vào model
            User = new StaffForCreation
            {
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                DateOfBirth = (DateTime)user.DateOfBirth,
                SelectedRole = selectedRole, // Gán vai trò hiện tại cho User.SelectedRole
                AvatarUrl = user.Avatar // Gán đường dẫn của avatar cũ
            };

            // Truyền danh sách các vai trò vào Razor Page
            Roles = roleNames;
            AvatarUrl = user.Avatar;


            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Redisplay the form if validation fails
                return Page();
            }
            // Nếu ảnh mới không được upload, giữ lại giá trị AvatarUrl cũ
            if (string.IsNullOrEmpty(AvatarUrl))
            {
                User.AvatarUrl = AvatarUrl;
            }

            // Lấy ClaimsPrincipal của người dùng đang đăng nhập
            var currentUser = HttpContext.User; // User hiện tại

            var result = await _staffService.UpdateStaff(User, currentUser);
            if (result.Succeeded)
            {
                Mess = "Update successful";

                // Truyền lại danh sách các vai trò vào Razor Page khi cập nhật thành công
                var roleNames = _roleManager.Roles.Select(role => role.Name).ToList();


                // Truyền danh sách các vai trò vào Razor Page
                Roles = roleNames;


                return Page(); // Quay lại trang update với dữ liệu cũ
            }

            // Nếu cập nhật không thành công, thêm lỗi vào ModelState và redisplay form
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // Truyền lại danh sách vai trò nếu có lỗi
            var roleNamesOnFailure = _roleManager.Roles.Select(role => role.Name).ToList();
            Roles = roleNamesOnFailure;

            return Page();
        }

    }
}
