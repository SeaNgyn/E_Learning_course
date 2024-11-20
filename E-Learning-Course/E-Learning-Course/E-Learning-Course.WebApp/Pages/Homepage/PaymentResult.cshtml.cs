using E_Learning_Course.Service;
using E_Learning_Course.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Learning_Course.WebApp.Pages.Homepage
{
    public class PaymentResultModel : PageModel
    {
        private readonly IPaymentService _paymentService;
        public string? Message { get; set; }
        public bool IsSuccessful { get; set; }

        public PaymentResultModel(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public async Task<IActionResult> OnGetAsync([FromQuery] IDictionary<string, string> paymentData)
        {
            // Collect all payment-related query parameters
            if (paymentData == null || !paymentData.ContainsKey("vnp_ResponseCode"))
            {
                Message = "Invalid payment data.";
                return Page();
            }


            // Verify the payment result with VNPay
            bool paymentVerified = await _paymentService.VerifyPaymentResultAsync(paymentData);

            if (paymentVerified)
            {
                Message = "Payments successful! Thank you for your purchase.";
                IsSuccessful = true;
            }
            else
            {
                Message = "Payments failed. Please try again or contact support.";
                IsSuccessful = false;
            }
            return Page();
        }
    }
}
