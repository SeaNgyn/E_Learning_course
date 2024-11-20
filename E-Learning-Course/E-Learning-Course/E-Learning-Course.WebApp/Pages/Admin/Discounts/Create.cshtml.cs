using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Learning_Course.WebApp.Pages.Admin.Discounts
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class CreateModel : PageModel
    {
        private IHostEnvironment _environment;
        private readonly IDiscountService _discountService;

        [BindProperty]
        public DiscountForCourse Discount { get; set; }
        public string? Mess { get; set; } = "";
        public string CurrentFilter { get; set; } = "";

        public List<Course> Courses { get; set; }


        public CreateModel(IHostEnvironment environment, IDiscountService discountService)
        {
            _discountService = discountService;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                Courses = context.Courses.ToList();
            }
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {

            await _discountService.AddDiscount(Discount);

            Mess = "Add ticket successfully";

            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                Courses = context.Courses.ToList();
            }
            return Page();
        }

    }
}
