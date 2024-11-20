using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Learning_Course.WebApp.Pages.Admin.Categories
{
    [Authorize(Roles = "Administrator")]
    public class UpdateModel : PageModel
    {
        private IHostEnvironment _environment;
        private readonly ICategoryService _catesService;

        [BindProperty]
        public CategoryForCreation Category { get; set; }
        public string? Mess { get; set; } = "";
        public string CurrentFilter { get; set; } = "";
        public UpdateModel(IHostEnvironment environment, ICategoryService cateService)
        {
            _catesService = cateService;
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
                var cate = context.Categories.Find(id);
                CategoryForCreation newCategory = new CategoryForCreation
                {
                    Id = cate.Id,
                    Name = cate.Name,
                    Description = cate.Description,
                    Status = cate.Status,
                };
                Category = newCategory;

                if (Category == null)
                {
                    return NotFound();
                }
            }
            return Page();

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}");
                    foreach (var err in error.Value.Errors)
                    {
                        Console.WriteLine($"Error: {err.ErrorMessage}");
                    }
                    Mess = "Data is not valid!";
                }
                return Page(); // Redisplay the form if validation fails
            }

            await _catesService.UpdateCategoryAsync(Category);

            TempData["SuccessMessage"] = "Category updated successfully.";

            return RedirectToPage("/Admin/Categories/List");
        }
    }
}
