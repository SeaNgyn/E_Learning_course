using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Learning_Course.WebApp.Pages.Homepage
{
    public class IndexModel : PageModel
    {
        public List<CourseList>? ProCourses { get; set; }
        public List<CourseList>? Courses { get; set; }
        public List<CourseList>? CoursesTrend { get; set; }
        private readonly ICourseService _courseService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, ICourseService courseService)
        {
            _logger = logger;
            _courseService = courseService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ProCourses = await _courseService.GetProCourses();
            Courses = await _courseService.GetCourseListHomePageFree();
            CoursesTrend = await _courseService.GetCourseTrend();
            return Page();
        }
    }
}
