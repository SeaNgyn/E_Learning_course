using E_Learning_Course.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IUserService
    {
        Task<PaginatedResult<StaffViewModel>> GetPagedUsersAsync(string searchByText, string sortBy, int showNumber, int currentPage);
    }
}
