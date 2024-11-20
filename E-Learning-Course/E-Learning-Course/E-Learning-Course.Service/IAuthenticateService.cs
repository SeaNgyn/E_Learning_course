using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IAuthenticateService
    {
        Task<IdentityResult> RegisterUser(UserForRegistration userForRegistration);
        Task<bool> ValidateUser(UserForAuthentication userForAuth);
        Task<string> CreateToken();
        Task<bool> IsEmailConfirmed(string email);
        Task<IList<string>> GetUserRoles(string email);
    }
}
