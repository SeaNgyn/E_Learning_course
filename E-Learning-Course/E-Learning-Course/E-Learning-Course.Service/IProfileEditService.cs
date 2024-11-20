using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace E_Learning_Course.Service
{
    public interface IProfileEditService
    {
        Task<bool> EditProfile(UserManager<User> _userManager, IFileService _fileService, IFormFile fileAvatar, UserEditDTO userEdit, User user);
    }
}
