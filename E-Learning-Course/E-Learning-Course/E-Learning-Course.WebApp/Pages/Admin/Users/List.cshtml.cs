using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;
namespace E_Learning_Course.WebApp.Pages.Admin.Users
{
    [Authorize(Roles = "Administrator")]
    public class ListModel : PageModel
    {
        public List<User> User { get; set; }
        public IPagedList<User> PagedUsers { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public string RoleUser { get; set; }
        public int? PageSize { get; set; }
        public int? PageNo { get; set; }

        public int? TotalPage { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ListModel(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> GetUserRoleAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? "No Role"; // Default to "No Role" if user has no roles
        }
        public List<string> RoleList { get; set; } // Role list for dropdown

        public async Task<IActionResult> OnGetAsync(int? pageNo, int? pageSize, string? searchTxt, string? sortBy, string? roleUser)
        {
            if (pageNo == null) pageNo = 1;
            if (pageSize == null) pageSize = 5;
            CurrentFilter = searchTxt ?? "";
            PageNo = pageNo;
            PageSize = pageSize;

            RoleList = await _roleManager.Roles.Select(r => r.Name).ToListAsync(); // Get all roles for the dropdown

            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                IQueryable<User> query = context.Users.OrderByDescending(x => x.CreatedAt);

                // Filter by role if specified
                if (!string.IsNullOrEmpty(roleUser))
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(roleUser);
                    var userIdsInRole = usersInRole.Select(u => u.Id).ToList();
                    query = query.Where(x => userIdsInRole.Contains(x.Id));
                }

                // Filter by search text
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(x => x.UserName.Contains(searchTxt));
                }

                // Sorting logic here
                if (!string.IsNullOrEmpty(sortBy))
                {
                    var sortParams = sortBy.Split('|');
                    string sortField = sortParams[0];
                    string sortDirection = sortParams[1];

                    query = sortField switch
                    {
                        "name" => sortDirection == "asc" ? query.OrderBy(c => c.UserName) : query.OrderByDescending(c => c.UserName),
                        _ => query
                    };
                }



                PagedUsers =  query.ToPagedList((int)pageNo, (int)pageSize);
            }
            return Page();
        }



    }
}
