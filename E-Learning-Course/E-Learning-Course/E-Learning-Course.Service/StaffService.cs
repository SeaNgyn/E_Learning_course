using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class StaffService : IStaffService
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private User? _user;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StaffService(RepositoryContext repositoryContext,
                            UserManager<User> userManager,
                            IConfiguration configuration,
                            RoleManager<IdentityRole> roleManager,
                            IFileService fileService,
                            IHttpContextAccessor httpContextAccessor)
        {
            _repositoryContext = repositoryContext;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _fileService = fileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityResult> AddStaff(StaffForCreation user, ClaimsPrincipal currentUser, string password)
        {
            // Lấy ID của người dùng đang đăng nhập
            //var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Sử dụng currentUser truyền vào
            //if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out Guid creatorId))
            //{
            //    return IdentityResult.Failed(new IdentityError { Description = "Invalid or missing creator user ID." });
            //}
            var userLogin = _httpContextAccessor.HttpContext.User;
            var userFind = await _userManager.FindByNameAsync(userLogin.Identity.Name);
            if (!Guid.TryParse(userFind.Id, out Guid creatorId))
                return IdentityResult.Failed(new IdentityError { Description = "Invalid creator user ID format." });
            var avatar = await _fileService.UploadAsync(user.Avatar);
            User newUser = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = 1,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = creatorId,
                Avatar = avatar.Blob.Uri.ToString()
            };

            // Tạo mật khẩu ngẫu nhiên
            var generatedPassword = password;

            // Tạo người dùng trong hệ thống
            var creationResult = await _userManager.CreateAsync(newUser, generatedPassword); 
            if (!creationResult.Succeeded)
                return creationResult;

            // Kiểm tra vai trò đã chọn có tồn tại không
            if (!await _roleManager.RoleExistsAsync(user.SelectedRole))
            {
                var roleCreationResult = await _roleManager.CreateAsync(new IdentityRole(user.SelectedRole));
                if (!roleCreationResult.Succeeded)
                    return roleCreationResult;
            }

            // Gán người dùng vào vai trò đã chọn
            var roleAssignmentResult = await _userManager.AddToRoleAsync(newUser, user.SelectedRole);
            if (!roleAssignmentResult.Succeeded)
                return roleAssignmentResult;


            return IdentityResult.Success; // Trả về thành công
        }

        public async Task<IdentityResult> UpdateStaff(StaffForCreation user, ClaimsPrincipal currentUser)
        {
            // Lấy ID của người dùng đang đăng nhập
            //var currentUserId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Sử dụng currentUser truyền vào
            //if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out Guid creatorId))
            //{
            //    return IdentityResult.Failed(new IdentityError { Description = "Invalid or missing creator user ID." });
            //}

            var userLogin = _httpContextAccessor.HttpContext.User;
            var userFind = await _userManager.FindByNameAsync(userLogin.Identity.Name);
            if (!Guid.TryParse(userFind.Id, out Guid creatorId))
                return IdentityResult.Failed(new IdentityError { Description = "Invalid creator user ID format." });
            //if(user.Avatar != null)
            //{
            //    var avatar = await _fileService.UploadAsync(user.Avatar);
            //}

            var updateUser = await _userManager.FindByEmailAsync(user.Email);
            updateUser.PhoneNumber = user.PhoneNumber;
            updateUser.Status = 1;
            updateUser.DateOfBirth = user.DateOfBirth;
            updateUser.UpdatedAt = DateTime.UtcNow;
            updateUser.UpdatedBy = creatorId;

            if (user.Avatar != null)
            {
                var avatar = await _fileService.UploadAsync(user.Avatar);
                updateUser.Avatar = avatar.Blob.Uri.ToString();
            }


            // Update the user with the new fields
            var updateResult = await _userManager.UpdateAsync(updateUser);
            if (!updateResult.Succeeded)
                return updateResult;

            // Lấy tất cả các vai trò hiện có của người dùng
            var currentRoles = await _userManager.GetRolesAsync(updateUser);

            // Xóa tất cả các vai trò hiện tại của người dùng
            var removeRolesResult = await _userManager.RemoveFromRolesAsync(updateUser, currentRoles);
            if (!removeRolesResult.Succeeded)
                return removeRolesResult;

            // Kiểm tra vai trò mới đã chọn có tồn tại không
            if (!await _roleManager.RoleExistsAsync(user.SelectedRole))
            {
                var roleCreationResult = await _roleManager.CreateAsync(new IdentityRole(user.SelectedRole));
                if (!roleCreationResult.Succeeded)
                    return roleCreationResult;
            }

            // Gán vai trò mới cho người dùng
            var roleAssignmentResult = await _userManager.AddToRoleAsync(updateUser, user.SelectedRole);
            if (!roleAssignmentResult.Succeeded)
                return roleAssignmentResult;

            return IdentityResult.Success; // Trả về thành công
        }


    }
}
