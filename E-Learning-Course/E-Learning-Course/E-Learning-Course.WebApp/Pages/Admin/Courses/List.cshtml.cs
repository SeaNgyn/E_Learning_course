using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using X.PagedList;
using X.PagedList.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace E_Learning_Course.WebApp.Pages.Admin.Courses
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class ListModel : PageModel
    {
        public List<Category> Categories { get; set; }
        public List<Course> Courses { get; set; }
        private readonly RepositoryContext _context;
        public IPagedList<Course> PagedCourses { get; set; }
        public string CurrentFilter { get; set; } = "";
        public string CurrentSort { get; set; }
        public int? PageSize { get; set; }
        public int? PageNo { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }
        public int? TotalPage { get; set; }
        public ListModel(RepositoryContext context)
        {
            _context = context;
        }
        public IActionResult OnGet(int? pageNo, int? pageSize, string? searchTxt, string? sortBy)
        {
            Categories =  _context.Categories.ToList();
            if (pageNo == null)
            {
                pageNo = 1;
            }
            if (pageSize == null)
            {
                pageSize = 5;
            }
            if (searchTxt != null)
            {
                CurrentFilter = searchTxt;
            }
            PageNo = pageNo;
            PageSize = pageSize;

            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                IQueryable<Course> query = context.Courses.Include(x => x.Category).OrderByDescending(c=>c.CreatedAt);
                if(User.IsInRole("Instructor"))
                {
                    query = query.Where(x => x.CreateBy == User.FindFirstValue(ClaimTypes.NameIdentifier));
                }    
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(x => (x.Title + x.Status + x.Category.Name).Contains(searchTxt));
                }
                if (CategoryId.HasValue)
                {
                    query = query.Where(c => c.CategoryId == CategoryId.Value);
                }

                // Sắp xếp
                if (!string.IsNullOrEmpty(sortBy))
                {
                    //var sort = (sortBy != null) ? CurrentSort : sortBy;

                    CurrentSort = sortBy;
                    var sortParams = sortBy.Split('|');
                    string sortField = sortParams[0];
                    string sortDirection = sortParams[1];

                    query = sortField switch
                    {
                        "name" => sortDirection == "asc" ? query.OrderBy(c => c.Title) : query.OrderByDescending(c => c.Title),
                        "price" => sortDirection == "asc" ? query.OrderBy(c => c.Price) : query.OrderByDescending(c => c.Price),
                        //"view" => sortDirection == "asc" ? query.OrderBy(c => c.ViewCount) : query.OrderByDescending(c => c.ViewCount),
                        "date" => sortDirection == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                        _ => query
                    };
                }
                PagedCourses = query.ToPagedList((int)pageNo, (int)pageSize);
                List<Course> listCourse = query.ToList();
                int morePage = listCourse.Count % pageSize != 0 ? 1 : 0;
                TotalPage = (int)(listCourse.Count / pageSize) + morePage;
            }
            return Page();
        }
    }
}
