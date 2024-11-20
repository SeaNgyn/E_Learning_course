using E_Learning_Course.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace E_Learning_Course.WebApp.Pages.Admin.Users
{
    public class BanModel : PageModel
    {
        private readonly UserManager<User> _userManager;

        public BanModel(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public string CurrentFilter { get; set; }

        [BindProperty]
        public string UserId { get; set; }

        public User User { get; set; }

        // Property to check if the user is banned
        public bool IsBanned => User?.Status == 0;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            User = await _userManager.FindByIdAsync(id);

            if (User == null)
            {
                return NotFound();
            }

            UserId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(UserId))
            {
                return NotFound();
            }

            // Find the user
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                return NotFound();
            }

            // Toggle user status: 1 for active, 0 for banned
            user.Status = user.Status == 1 ? 0 : 1;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Redirect to user list after the update
                return RedirectToPage("./List");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
