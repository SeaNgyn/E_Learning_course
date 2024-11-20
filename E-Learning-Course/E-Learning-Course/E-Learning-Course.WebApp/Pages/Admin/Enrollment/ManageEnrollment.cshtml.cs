using E_Learning_Course.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Learning_Course.WebApp.Pages.Admin.Enrollment
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class ManageEnrollmentModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        public List<EnrollmentDto> enrollmentDtos;
        public ManageEnrollmentModel(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
            enrollmentDtos = new List<EnrollmentDto>();
        }
        public async Task<IActionResult> OnGet(int id)
        {
            enrollmentDtos = await _enrollmentService.EnrollmentListByCourseId(id);
            return Page();
        }
    }
}
