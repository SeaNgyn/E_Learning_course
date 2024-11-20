using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PaginatedResult<StaffViewModel>> GetPagedUsersAsync(string searchByText, string sortBy, int showNumber, int currentPage)
    {

        // Get all users
        var users =  _userManager.Users.ToList();

        // Filter by search text
        if (!string.IsNullOrEmpty(searchByText))
        {
            users = users.Where(u => u.UserName.Contains(searchByText, StringComparison.OrdinalIgnoreCase)).ToList();
        }


        IEnumerable<User> sortedUsers = sortBy switch
        {
            "name" => users.OrderBy(u => u.UserName),
            _ => users 
        };

        // Paginate the results
        var paginatedUsers = sortedUsers
            .Skip((currentPage - 1) * showNumber)
            .Take(showNumber)
            .ToList();

        var totalCount =  sortedUsers.Count();

        // Map IdentityUser to UserViewModel
        var userViewModels =  paginatedUsers.Select( u => new StaffViewModel
        {
            Id = u.Id,
            UserName = u.UserName,
            Role = string.Join(", ",  _userManager.GetRolesAsync(u)), 
            Status = u.Status,
            Avatar = "https://example.com/default_profile.jpg" 
        }).ToList();

        return new PaginatedResult<StaffViewModel>
        {
            Items = userViewModels,
            PageNumber = currentPage,
            PageSize = showNumber,
            TotalCount = totalCount
        };
    }
}
