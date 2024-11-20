using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using E_Learning_Course.Data.Entities;

namespace E_Learning_Course.WebApp.Pages.Authentication
{
    public class LogoutAdminModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LogoutAdminModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await _signInManager.SignOutAsync();
            if (Request.Cookies["jwtToken"] != null)
            {
                Response.Cookies.Delete("jwtToken");
            }
            return RedirectToPage("/Authentication/Login");
        }
    }
}