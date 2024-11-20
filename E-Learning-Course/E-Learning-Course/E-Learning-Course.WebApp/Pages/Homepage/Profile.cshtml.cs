using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace E_Learning_Course.WebApp.Pages.Homepage
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly IEnrollmentService _enrollmentService;
        public string Role { get; set; }
        public User GetUser { get; set; }
        public List<Enrollment> Enrollments { get; set; }

        public ProfileModel(UserManager<User> userManager, IEnrollmentService enrollmentService)
        {
            _userManager = userManager;
            _enrollmentService = enrollmentService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var name = User.Identity?.Name;
            if (name == null)
            {
                return RedirectToPage("../Error");
            }

            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
            {
                return RedirectToPage("../Error");
            }

            GetUser = user;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToPage("../Error");
            }

            string userId = userIdClaim.Value;
            Enrollments = await _enrollmentService.GetListCustomerEnrollment(userId);

            IList<string> roles = await _userManager.GetRolesAsync(user);
            Role = roles.FirstOrDefault() ?? "No Role";

            return Page();
        }
    }
}
