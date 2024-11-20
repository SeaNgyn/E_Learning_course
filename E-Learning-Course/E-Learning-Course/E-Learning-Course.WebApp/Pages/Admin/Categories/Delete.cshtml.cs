using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Learning_Course.WebApp.Pages.Admin.Categories
{
    [Authorize(Roles = "Administrator")]
    public class DeleteModel : PageModel
    {
        private readonly ICategoryService _cateService;

        public DeleteModel(ICategoryService cateService)
        {
            _cateService = cateService;
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
                if (cate == null)
                {
                    return NotFound();
                }

                // Check the current status and update accordingly
                if (cate.Status == 1) // Active
                {
                    cate.Status = 0; // Set to Inactive (ban)
                    TempData["SuccessMessage"] = "Category banned successfully.";
                }
                else if (cate.Status == 0) // Inactive
                {
                    cate.Status = 1; // Set to Active (unban)
                    TempData["SuccessMessage"] = "Category unbanned successfully.";
                }

                context.Categories.Update(cate);
                context.SaveChanges();
            }

            return RedirectToPage("/Admin/Categories/List");
        }
    }
}
