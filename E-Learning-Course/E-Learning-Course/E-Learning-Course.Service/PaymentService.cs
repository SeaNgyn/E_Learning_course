using E_Learning_Course.Data;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.Utilities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly RepositoryContext _context;
        private readonly string _tmnCode = "";
        private readonly string _hashSecret = "";
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public PaymentService(RepositoryContext context, IHttpContextAccessor contextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
        }

        public string CreatePaymentUrl(CourseTransaction course)
        {
            string url = "http://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string returnUrl = $"http://localhost:5059/Homepage/PaymentResult?customerID={course.CustomerId}&id={course.CourseId}";  // Pass customer Id



            long paymentAmount = course.ReturnBack ? (long)(course.Price * 50) : (long)(course.Price * 100);
            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", _tmnCode);
            pay.AddRequestData("vnp_Amount", (paymentAmount.ToString()));
            pay.AddRequestData("vnp_BankCode", "VNBANK"); 
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", course.Ip);
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);
            pay.AddRequestData("vnp_TxnRef", "HD_" + DateTime.Now.Ticks.ToString());

            string paymentUrl = pay.CreateRequestUrl(url, _hashSecret);

            return paymentUrl;
        }

        public async Task<bool> VerifyPaymentResultAsync(IDictionary<string, string> paymentData)
        {
            var payLib = new PayLib();
            foreach (var key in paymentData.Keys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    payLib.AddResponseData(key, paymentData[key]);
                }
            }

            string inputHash = paymentData["vnp_SecureHash"];
            bool isValid = payLib.ValidateSignature(inputHash, _hashSecret);

            if (!isValid)
            {
                return false; // Signature is invalid
            }

            string responseCode = paymentData["vnp_ResponseCode"];
            if (responseCode == "00")
            {
                string transactionId = paymentData["vnp_TransactionNo"];
                double amount = double.Parse(paymentData["vnp_Amount"]) / 100;
                string customerId = paymentData["customerID"];
                int courseId = int.Parse(paymentData["id"]);

                // Check if the enrollment status is 2 (completed)
                var enrollmentStatus = await _context.Enrollments
                    .Where(x => x.CourseId == courseId && x.UserId == customerId)
                    .Select(x => x.Status)
                    .FirstOrDefaultAsync();

                bool shouldReturnBack = enrollmentStatus == 2;

                var payment = new Payment
                {
                    TransactionId = transactionId,
                    Amount = (decimal)amount,
                    PaymentDate = DateTime.Now,
                    CourseId = int.Parse(paymentData["id"]),
                    UserId = paymentData["customerID"],  // Save customer Id
                    IsSuccessful = true,
                    Status = 1,
                    PaymentMethod = "VNPAY"
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                if (shouldReturnBack)
                {
                    await UpdateEnrollmentStatus(customerId, courseId);
                }
                else
                {
                    await GrantCourseAccessToUser(paymentData["customerID"], int.Parse(paymentData["id"]));
                }

                return true; // Payment succeeded
            }

            return false; // Payment failed
        }

        public async Task GrantCourseAccessToUser(string userId, int courseId)
        {
            var limitDate = await _context.Courses.Where(x=>x.Id==courseId).Select(x => x.LimitDay).FirstOrDefaultAsync();
            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                Progress = 0,
                Status = 1,
                ExpiredDate = DateTime.Now.AddDays((double)limitDate)
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEnrollmentStatus(string userId, int courseId)
        {
            var limitDate = await _context.Courses.Where(x => x.Id == courseId).Select(x => x.LimitDay).FirstOrDefaultAsync();
            var enrollment = await _context.Enrollments
                .Where(x => x.CourseId == courseId && x.UserId == userId)
                .FirstOrDefaultAsync();
            enrollment.Status = 1;
            enrollment.ExpiredDate = DateTime.Now.AddDays((double)limitDate / 2);

            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }


        public async Task<List<PaymentList>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Select (p => new PaymentList
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    TransactionId = p.TransactionId,
                    IsSuccessful = p.IsSuccessful,
                    CourseId = p.CourseId,
                    UserId = p.UserId,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status
                })
                .ToListAsync ();
        }

        public async Task<List<PaymentList>> SearchPaymentsAsync(DateTime? fromDate, DateTime? toDate, string orderNumber ,int? status)
        {
            var query = _context.Payments.AsQueryable ();

            if ( fromDate.HasValue ) {
                query = query.Where (p => p.PaymentDate >= fromDate.Value);
            }

            if ( toDate.HasValue ) {
                query = query.Where (p => p.PaymentDate <= toDate.Value);
            }

            if ( !string.IsNullOrEmpty (orderNumber) ) {
                query = query.Where (p => p.TransactionId.Contains (orderNumber));
            }

            if ( status.HasValue ) {
                query = query.Where (p => p.Status == status.Value);
            }

            return await query.Select (p => new PaymentList
            {
                Id = p.Id,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                TransactionId = p.TransactionId,
                IsSuccessful = p.IsSuccessful,
                CourseId = p.CourseId,
                UserId = p.UserId,
                PaymentMethod = p.PaymentMethod,
                Status = p.Status
            }).ToListAsync ();
        }
    }


}
