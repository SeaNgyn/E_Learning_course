using E_Learning_Course.Data.Entities;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace E_Learning_Course.WebApp.Pages.Admin.Categories
{
    [Authorize(Roles = "Administrator")]
    public class ListModel : PageModel
    {
        public List<Category> Categories { get; set; }

        public IPagedList<Category> PagedCategories { get; set; }

        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int? PageSize { get; set; }
        public int? PageNo { get; set; }

        public int? TotalPage { get; set; }
        public IActionResult OnGet(int? pageNo, int? pageSize, string? searchTxt, string? sortBy)
        {
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
                IQueryable<Category> query = context.Categories;
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(x => (x.Name + x.Description).Contains(searchTxt));
                }
                // S?p x?p
                if (!string.IsNullOrEmpty(sortBy))
                {
                    //var sort = (sortBy != null) ? CurrentSort : sortBy;

                    CurrentSort = sortBy;
                    var sortParams = sortBy.Split('|');
                    string sortField = sortParams[0];
                    string sortDirection = sortParams[1];

                    query = sortField switch
                    {
                        "name" => sortDirection == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
                        _ => query
                    };
                }
                PagedCategories = query.ToPagedList((int)pageNo, (int)pageSize);
                List<Category> listCate = query.ToList();
                int morePage = (listCate.Count % pageSize != 0 ? 1 : 0);
                TotalPage = (int)(listCate.Count / pageSize) + morePage;
            }
            return Page();
        }

    }

}
