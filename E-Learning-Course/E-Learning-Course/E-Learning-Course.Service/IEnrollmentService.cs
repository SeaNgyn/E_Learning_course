using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IEnrollmentService
    {
        Task<List<Enrollment>> GetListCustomerEnrollment(string userId);
        Task<PaginatedResult<Enrollment>> GetEnrollmentsAsync(string searchTerm, string sortField, bool isAscending, int page, int pageSize,string userId);
        Task<List<Enrollment>> EnrollmentList();
        int AmountEnroll(int courseId);
        Task<List<EnrollmentDto>> EnrollmentListByCourseId(int courseId);
    }
}

