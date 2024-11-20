using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class EditProfileService : IProfileEditService
    {
        public async Task<bool> EditProfile(UserManager<User> _userManager, IFileService _fileService, IFormFile fileAvatar, UserEditDTO userEdit, User user)
        {
            bool hasChanges = false;
            if (fileAvatar != null)
            {
                var uploadAvatar = await _fileService.UploadAsync(fileAvatar);
                user.Avatar = uploadAvatar.Blob.Uri.ToString();
                hasChanges = true;
            }

            if (user.FirstName != userEdit.FirstName)
            {
                user.FirstName = userEdit.FirstName;
                hasChanges = true;
            }

            if (user.LastName != userEdit.LastName)
            {
                user.LastName = userEdit.LastName;
                hasChanges = true;
            }

            if (user.PhoneNumber != userEdit.PhoneNumber)
            {
                user.PhoneNumber = userEdit.PhoneNumber;
                hasChanges = true;
            }

            if (hasChanges)
            {
                var editResult = await _userManager.UpdateAsync(user);
                if (editResult.Succeeded)
                {
                    if (user.FirstName != userEdit.FirstName)
                    {
                        await _userManager.RemoveClaimAsync(user, new Claim("FirstName", user.FirstName ?? ""));
                        await _userManager.AddClaimAsync(user, new Claim("FirstName", user.FirstName ?? ""));
                    }

                    if (user.LastName != userEdit.LastName)
                    {
                        await _userManager.RemoveClaimAsync(user, new Claim("LastName", user.LastName ?? ""));
                        await _userManager.AddClaimAsync(user, new Claim("LastName", user.LastName ?? ""));
                    }

                    if (fileAvatar != null)
                    {
                        await _userManager.RemoveClaimAsync(user, new Claim("Avatar", user.Avatar ?? ""));
                        await _userManager.AddClaimAsync(user, new Claim("Avatar", user.Avatar ?? ""));
                    }
                }
                return true;
            }
            return false;
        }
    }
}
