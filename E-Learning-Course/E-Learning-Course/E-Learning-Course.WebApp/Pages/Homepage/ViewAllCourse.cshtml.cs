using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace E_Learning_Course.WebApp.Pages.Homepage
{
    public class ViewAllCourseModel : PageModel
    {
        public List<Course> Courses { get; set; }

        public List<Category> Categories { get; set; }
        public IPagedList<Course> PagedCourses { get; set; }

        //public string CurrentFilter { get; set; } = "";
        public string CurrentSort { get; set; }
        public int? PageSize { get; set; }
        public int? PageNo { get; set; }

        public int? TotalPage { get; set; }

        // Các giá tr? filter ???c binding t? ??ng t? URL
        [BindProperty(SupportsGet = true)]
        public int? Price { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Category { get; set; }

        public IActionResult OnGet(int? pageNo, int? pageSize, string? searchTxt, string? sortBy, int? category, int? price)
        {
            pageNo ??= 1;  // Nếu không có giá trị, mặc định là 1
            pageSize ??= 5;  // Mặc định là 5 (có thể tùy chỉnh theo yêu cầu)

            PageNo = pageNo;
            PageSize = pageSize;

            //CurrentFilter = searchTxt ?? "";
            CurrentSort = sortBy;
            Category = category;
            Price = price;

            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                IQueryable<Course> query = context.Courses.Include(x => x.Category);

                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(x => (x.Title + x.Status + x.Category.Name).Contains(searchTxt));
                }

                // Áp dụng sắp xếp
                if (!string.IsNullOrEmpty(CurrentSort))
                {
                    var sortParams = CurrentSort.Split('|');
                    string sortField = sortParams[0];
                    string sortDirection = sortParams[1];

                    query = sortField switch
                    {
                        "name" => sortDirection == "asc" ? query.OrderBy(c => c.Title) : query.OrderByDescending(c => c.Title),
                        "price" => sortDirection == "asc" ? query.OrderBy(c => c.Price) : query.OrderByDescending(c => c.Price),
                        "date" => sortDirection == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                        _ => query
                    };
                }

                if (Category.HasValue)
                {
                    query = query.Where(x => x.CategoryId == Category.Value);
                }

                if (Price.HasValue)
                {
                    query = Price == 1 ? query.Where(x => x.Price > 0) : query.Where(x => x.Price == 0);
                }

                PagedCourses = query.ToPagedList((int)pageNo, (int)pageSize);
                List<Course> listCourse = query.ToList();
                int morePage = listCourse.Count % pageSize != 0 ? 1 : 0;
                TotalPage = (int)(listCourse.Count / pageSize) + morePage;
                Categories = context.Categories.ToList();
            }
            return Page();
        }


    }
}

