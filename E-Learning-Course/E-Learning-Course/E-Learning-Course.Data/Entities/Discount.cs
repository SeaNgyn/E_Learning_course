using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Data.Entities
{
    public class Discount
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public decimal DiscountPer { get; set; }
        public int MaxUses { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        // Foreign keys
        public string CreateBy { get; set; }
        public string? UpdateBy { get; set; }

        public Course Course { get; set; }
        public int CourseId { get; set; }

    }
}
