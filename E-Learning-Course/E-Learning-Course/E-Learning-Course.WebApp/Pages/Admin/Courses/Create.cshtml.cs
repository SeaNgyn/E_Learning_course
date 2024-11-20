using E_Learning_Course.ViewModels;
using E_Learning_Course.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;

namespace E_Learning_Course.WebApp.Pages.Admin.Courses
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class CreateModel : PageModel
    {
        private readonly ICourseService _courseService;

        [BindProperty]
        public CourseForCreation CourseForCreation { get; set; }

        public string? Mess { get; set; } = "";

        public List<Category> Categories { get; set; }

        public CreateModel(ICourseService courseService)
        {
            _courseService = courseService;

            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                Categories = context.Categories.Where(x=>x.Status==1).ToList();
            }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var success = await _courseService.CreateCourseAsync(CourseForCreation);

                if (success)
                {
                    TempData["ToastMessage"] = "Thêm khóa h?c thành công";
                    TempData["ToastType"] = "success";
                    return Page(); // Navigate to another page after success
                }
                else
                {
                    Mess = "An error occurred while creating the course.";
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
