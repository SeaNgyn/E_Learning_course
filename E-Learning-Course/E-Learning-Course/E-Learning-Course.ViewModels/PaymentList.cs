using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class PaymentList
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionId { get; set; }
        public bool IsSuccessful { get; set; }
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public string PaymentMethod { get; set; }
        public int Status { get; set; }
    }
}
