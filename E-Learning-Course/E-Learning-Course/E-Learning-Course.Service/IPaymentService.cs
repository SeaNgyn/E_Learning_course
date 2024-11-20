using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IPaymentService
    {
        string CreatePaymentUrl(CourseTransaction course);
        Task<bool> VerifyPaymentResultAsync(IDictionary<string, string> paymentData);
        Task GrantCourseAccessToUser(string userId, int courseId);
        Task<List<PaymentList>> GetAllPaymentsAsync();
        Task<List<PaymentList>> SearchPaymentsAsync(DateTime? fromDate, DateTime? toDate, string orderNumber , int? status);

    }
}
