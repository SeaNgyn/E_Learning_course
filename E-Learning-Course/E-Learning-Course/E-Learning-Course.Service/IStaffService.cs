using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IStaffService
    {
        Task<IdentityResult> AddStaff(StaffForCreation user, ClaimsPrincipal currentUser, string password);

        Task<IdentityResult> UpdateStaff(StaffForCreation user, ClaimsPrincipal currentUser);
    }
}
