using E_Learning_Course.Data;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace E_Learning_Course.WebApp.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class DashboardModel : PageModel
    {
        private readonly RepositoryContext _context;
        // Properties for statistics
        public int TotalCourses { get; set; }
        public int ActiveUsers { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalFee { get; set; }
        public int NewCoursesThisMonth { get; set; }
        [BindProperty]
        public int Month { get; set; } = DateTime.Now.Month;
        [BindProperty]
        public int Year { get; set; } = DateTime.Now.Year;

        // Data for charts
        public List<string> EnrollmentsChartLabels { get; set; }
        public List<int> EnrollmentsChartData { get; set; }

        public List<string> RevenueChartLabels { get; set; }
        public List<decimal> RevenueChartData { get; set; }

        // Recent activities
        public List<Activity> RecentActivities { get; set; }
        private readonly IFinanceService _financeService;
        public List<string> FinanceChartLabels { get; set; }
        public List<double> FinanceChartData { get; set; }
        public DashboardModel(IFinanceService financeService,RepositoryContext context)

        {
            _financeService = financeService;
            _context = context;
        }
        public async Task OnGetAsync(int month,int year)
        {
            if(month == 0)
            {
                month = DateTime.Now.Month;
            }
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            // Fetch data from your database or services
            // For demonstration, we'll use dummy data

            TotalCourses = await _context.Courses.CountAsync();
            ActiveUsers = await _context.Users.Where(x => x.Status == 1).CountAsync();
            TotalRevenue = await _financeService.GetRevenueAsync(month, year);
            TotalFee = await _financeService.GetFeesAsync(month, year);
            NewCoursesThisMonth = await _context.Courses.Where(x => x.CreatedAt.Month == DateTime.Now.Month).CountAsync();

            // Enrollments over the past 12 months
            EnrollmentsChartLabels = new List<string>();
            EnrollmentsChartData = new List<int>();
            for (int i = 1; i <= DateTime.Now.Month; i++)
            {
                // Format the month as a label (e.g., "Jan", "Feb", etc.)
                EnrollmentsChartLabels.Add(new DateTime(year, i, 1).ToString("MMM"));

                // Replace this with actual data fetching logic if available
                EnrollmentsChartData.Add(await _context.Enrollments
                    .Where(e => e.EnrollmentDate.Month == i && e.EnrollmentDate.Year == year)
                    .CountAsync());
            }

            // Revenue breakdown by course category
            RevenueChartLabels = new List<string> { "Web Development", "Data Science", "Design", "Marketing" };
            RevenueChartData = new List<decimal> { 5000m, 3000m, 2000m, 2500m };

            // Recent activities
            RecentActivities = new List<Activity>
            {
                new Activity { Timestamp = DateTime.Now.AddMinutes(-10), Description = "Added new course: ASP.NET Core" },
                new Activity { Timestamp = DateTime.Now.AddHours(-2), Description = "User JohnDoe enrolled in 'React - The Complete Guide'" },
                new Activity { Timestamp = DateTime.Now.AddDays(-1), Description = "Updated course: Tailwind CSS From Scratch" },
                // Add more activities as needed
            };
            FinanceChartLabels = new List<string>();
            FinanceChartData = new List<double>();

            for (int i = 1; i < DateTime.Now.Month; i++)
            {
                string labelMonth = new DateTime(year, i, 1).ToString("MMM");
                FinanceChartLabels.Add(labelMonth);

                // Fetch revenue and fees for each month
                var revenue = await _financeService.GetRevenueAsync(i, year);
                var fees = await _financeService.GetFeesAsync(i, year);

                // Calculate net profit
                FinanceChartData.Add(revenue - fees);
            }
        }


        public async Task<IActionResult> OnPostUpdateRevenueAsync()
        {

            var revenue = await _financeService.CalculateRevenueAsync(Month, Year);
            var finance = new FinanceForUpdate()
            {
                Month = Month,
                Year = Year,
                Revenue = revenue
            };
            await _financeService.UpdateOrCreateFinanceRecordAsync(finance);

            await OnGetAsync(Month, Year);

            return Page();
        }

        public class Activity
        {
            public DateTime Timestamp { get; set; }
            public string Description { get; set; }
        }
    }
}
