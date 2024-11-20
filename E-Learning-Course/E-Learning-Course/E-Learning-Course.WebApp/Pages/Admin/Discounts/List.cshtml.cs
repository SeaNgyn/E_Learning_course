using E_Learning_Course.Data.Entities;
using E_Learning_Course.WebApp.ContextFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace E_Learning_Course.WebApp.Pages.Admin.Discounts
{
    [Authorize(Roles = "Administrator, Instructor")]
    public class ListModel : PageModel
    {
        public List<Discount> Discounts { get; set; }
        public IPagedList<Discount> PagedDiscount { get; set; }
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
                pageSize = 1;
            }
            if (searchTxt != null)
            {
                CurrentFilter = searchTxt;
            }
            PageNo = pageNo;
            PageSize = pageSize;

            using (var context = new RespositoryContextFactory().CreateDbContext(null))
            {
                IQueryable<Discount> query = context.Discounts;
                if (!string.IsNullOrEmpty(searchTxt))
                {
                    query = query.Where(x => (x.Code).Contains(searchTxt));
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
                        "name" => sortDirection == "asc" ? query.OrderBy(c => c.Code) : query.OrderByDescending(c => c.Code),
                        "date" => sortDirection == "asc" ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                        _ => query
                    };
                }


                PagedDiscount = query.ToPagedList((int)pageNo, (int)pageSize);
                List<Discount> listDiscount = query.ToList();
                int morePage = (listDiscount.Count % pageSize != 0 ? 1 : 0);
                TotalPage = (int)(listDiscount.Count / pageSize) + morePage;

            }
            return Page();
        }
    }
}
