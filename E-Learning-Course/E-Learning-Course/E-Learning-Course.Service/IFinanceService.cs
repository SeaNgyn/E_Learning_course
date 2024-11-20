using E_Learning_Course.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IFinanceService
    {
        Task<double> CalculateRevenueAsync(int month, int year);
        Task UpdateOrCreateFinanceRecordAsync(FinanceForUpdate finance);
        Task<double> GetRevenueAsync(int month, int year);
        Task<double> GetFeesAsync(int month, int year);
    }
}
