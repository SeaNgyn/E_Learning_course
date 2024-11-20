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
    public class CreateModel : PageModel
    {
        private IHostEnvironment _environment;
        private readonly ICategoryService _catesService;

        [BindProperty]
        public CategoryForCreation? Category { get; set; }
        public string? Mess { get; set; } = "";
        public string CurrentFilter { get; set; } = "";



        public CreateModel(IHostEnvironment environment, ICategoryService cateService)
        {
            _catesService = cateService;
            _environment = environment;
        }

        public IActionResult OnGet()
        {
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Mess = "Data is not valid!";
                return Page();
            }

            await _catesService.AddCategoryAsync(Category);

            TempData["SuccessMessage"] = "Category added successfully.";

            return RedirectToPage("/Admin/Categories/List");
        }
    }
}
