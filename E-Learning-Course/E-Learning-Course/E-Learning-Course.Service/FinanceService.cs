using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class FinanceService : IFinanceService
    {
        private readonly RepositoryContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        public FinanceService(RepositoryContext context,IHttpContextAccessor httpContextAccessor,UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<double> CalculateRevenueAsync(int month, int year)
        {
            // Calculate total revenue by summing successful payments for the given month and year
            return (double)await _context.Payments
                .Where(p => p.PaymentDate.Month == month && p.PaymentDate.Year == year && p.IsSuccessful)
                .SumAsync(p => p.Amount);
        }
        public async Task UpdateOrCreateFinanceRecordAsync(FinanceForUpdate finance)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userLogin = await _userManager.FindByNameAsync(user.Identity.Name);
            // Check if there's already a record for the specified month and year
            var financeRecord = await _context.Finances
                .FirstOrDefaultAsync(f => f.Month == finance.Month && f.Year == finance.Year);

            if (financeRecord != null)
            {
                // Update the existing record
                financeRecord.Revenue = finance.Revenue;
                financeRecord.UpdatedAt = DateTime.Now;
                financeRecord.UpdatedBy = userLogin.Id;
            }
            else
            {
                // Create a new record
                var newFinanceRecord = new Finance
                {
                    Month = finance.Month,
                    Year = finance.Year,
                    Revenue = finance.Revenue,
                    Type = "Course",
                    CreatedAt = DateTime.Now,
                    CreatedBy = userLogin.Id,
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = userLogin.Id
                };

                await _context.Finances.AddAsync(newFinanceRecord);
            }

            await _context.SaveChangesAsync();
        }
        public async Task<double> GetRevenueAsync(int month, int year)
        {
            return (double)await _context.Finances
                .Where(p => p.Month == month && p.Year == year)
                .SumAsync(p => p.Revenue);
        }

        // Calculates finance fees based on a fixed percentage or a flat fee per successful payment
        public async Task<double> GetFeesAsync(int month, int year)
        {
            var sum = await _context.Finances
                .Where(f => f.Month == month && f.Year == year)
                .SumAsync(f => (double?)f.Fee); // SumAsync returns nullable

            return sum ?? 0; // Return 0 if sum is null
        }

    }
}
