using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Learning_Course.WebApp.Pages.Homepage
{
    public class PaymentConfirmModel : PageModel
    {
        public readonly ICourseService _courseService;
        public CoursePayment? Course;
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }
        public PaymentConfirmModel(ICourseService courseService)
        {
            _courseService = courseService;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                ReturnUrl = Url.Page("/Homepage/PaymentConfirm", new { id = id });
                // Redirect to the login page if the user is not authenticated
                return RedirectToPage("/Authentication/Login");
            }
            Course = await _courseService.GetCoursePayment(id, HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
            if (Course == null)
            {
                // Handle the case where the payment details were not found
                return NotFound();
            }

            // If needed, you can return to a view for confirmation rather than redirecting
            return Page(); // Return the confirmation page
        }
    }
}
