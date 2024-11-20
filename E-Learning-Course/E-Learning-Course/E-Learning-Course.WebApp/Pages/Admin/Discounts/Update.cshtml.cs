using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Learning_Course.WebApp.Pages.Admin.Discounts
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class UpdateModel : PageModel
    {
        private IHostEnvironment _environment;
        private readonly IDiscountService _discountService;

        [BindProperty]
        public DiscountForCourse Discount { get; set; }
        public string? Mess { get; set; } = "";
        public string CurrentFilter { get; set; } = "";
        public List<Course> Courses = new List<Course>();
        public UpdateModel(IHostEnvironment environment, IDiscountService discountService)
        {
            _discountService = discountService;
            _environment = environment;
        }
        public IActionResult OnGet(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                var discount = context.Discounts.Find(id);
                DiscountForCourse newDiscount = new DiscountForCourse
                {
                    Id = id,
                    Code = discount.Code,
                    DiscountPer = discount.DiscountPer,
                    MaxUses = discount.MaxUses,
                    StartDate = discount.StartDate,
                    EndDate = discount.EndDate,
                    CourseName = discount.CourseId
                };
                Discount = newDiscount;

                if (Discount == null)
                {
                    return NotFound();
                }
                Courses = context.Courses.ToList();
            }
            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Kiểm tra lỗi
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}");
                    foreach (var err in error.Value.Errors)
                    {
                        Console.WriteLine($"Error: {err.ErrorMessage}");
                    }
                }
                return Page(); // Redisplay the form if validation fails
            }


            // Lấy ClaimsPrincipal của người dùng đang đăng nhập
            var currentUser = HttpContext.User; // User ở đây là ClaimsPrincipal của người đang đăng nhập   
            await _discountService.UpdateDiscount(Discount, currentUser);


            Mess = "Update ticket successfully";

            return Page();
        }
    }
}
