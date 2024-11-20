using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Learning_Course.WebApp.Pages.Admin.Courses
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class UpdateModel : PageModel
    {
        private readonly ICourseService _courseService;

        [BindProperty]
        public CourseForUpdate? CourseForUpdate { get; set; }

        public string? Mess { get; set; } = "";
        public List<Category>? Categories { get; set; }
        public CourseDetail? CourseDetail { get; set; }

        public UpdateModel(ICourseService courseService)
        {
            _courseService = courseService;
            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                Categories = context.Categories.Where(x => x.Status == 1).ToList();
            }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            CourseDetail = await _courseService.GetCourseDetailHomePage(id);

            if (CourseDetail == null)
            {
                Mess = "Course not found.";
                return RedirectToPage("/Admin/Courses/List");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            CourseDetail = await _courseService.GetCourseDetailHomePage(id);
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var success = await _courseService.UpdateCourseAsync(CourseForUpdate);

                if (success)
                {
                    TempData["SuccessMessage"] = "Course updated successfully.";
                    return RedirectToPage("/Admin/Courses/List");
                }
                else
                {
                    Mess = "An error occurred while updating the course.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Mess = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}
