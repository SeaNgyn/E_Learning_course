using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Data.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int CategoryId { get; set; }
        public double Price { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }

        //co the null
        public TimeSpan? Duration { get; set; }
        public string Status { get; set; }
        public string PreviewVideo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public int? LimitDay { get; set; }
        // Foreign keys
        public string CreateBy { get; set; }

        //co the null
        public string? UpdateBy { get; set; }

        // Navigation properties
        public User Creator { get; set; }
        public User Updater { get; set; }
        public Category Category { get; set; }

        // Add this property for the one-to-many relationship
        public ICollection<Discount> Discounts { get; set; } = new List<Discount> ();

    }
}
