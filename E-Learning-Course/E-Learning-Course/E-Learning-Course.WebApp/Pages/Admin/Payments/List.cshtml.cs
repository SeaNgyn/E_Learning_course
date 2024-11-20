using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;
using X.PagedList.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_Learning_Course.WebApp.Pages.Admin.Payments
{
    [Authorize(Roles = "Administrator")]
    public class ListModel : PageModel
    {
        private readonly IPaymentService _paymentService;

        public ListModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public List<PaymentList> Payments { get; set; }


        public IPagedList<PaymentList> PagedPayments { get; set; }

        public string CurrentFilter { get; set; } = "";
        public string CurrentSort { get; set; }
        public int? PageSize { get; set; }
        public int? PageNo { get; set; }

        public int? TotalPage { get; set; }


        public async Task OnGetAsync(DateTime? fromDate, DateTime? toDate, string orderNumber, int? status, int? pageNo, int? pageSize)
        {
            pageNo ??= 1;
            pageSize ??= 5;

            var paymentsQuery = await _paymentService.SearchPaymentsAsync (fromDate, toDate, orderNumber, status);

            PagedPayments = paymentsQuery.ToPagedList ((int)pageNo, (int)pageSize);

            int totalItems = paymentsQuery.Count ();
            int morePage = totalItems % pageSize != 0 ? 1 : 0;
            TotalPage = ( totalItems / pageSize ) + morePage;

            PageNo = pageNo;
            PageSize = pageSize;
        }

    }
}

