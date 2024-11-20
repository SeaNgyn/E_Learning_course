using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Learning_Course.Data.Entities;
using System.Threading.Tasks;
using E_Learning_Course.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace E_Learning_Course.WebApp.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class UpdateFeeModel : PageModel
    {
        private readonly RepositoryContext _context;

        public UpdateFeeModel(RepositoryContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int Month { get; set; }

        [BindProperty]
        public int Year { get; set; }

        [BindProperty]
        public double Fee { get; set; }

        [BindProperty]
        public string? Description { get; set; }

        public async Task<IActionResult> OnGetAsync(int month, int year)
        {
            // Load the existing fee record
            var finance = await _context.Finances
                .FirstOrDefaultAsync(f => f.Month == month && f.Year == year);

            if (finance == null)
            {
                return NotFound();
            }

            Month = finance.Month;
            Year = finance.Year;
            Fee = finance.Fee;
            Description = finance.Description;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Find and update the finance record
            var finance = await _context.Finances
                .FirstOrDefaultAsync(f => f.Month == Month && f.Year == Year);

            if (finance == null)
            {
                return NotFound();
            }

            finance.Fee = Fee;
            finance.Description = Description;
            finance.UpdatedAt = DateTime.Now;
            finance.UpdatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Admin/Dashboard");
        }
    }
}
