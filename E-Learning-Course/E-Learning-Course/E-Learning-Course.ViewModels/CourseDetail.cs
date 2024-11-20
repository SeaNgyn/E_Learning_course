using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class CourseDetail
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? CategoryName { get; set; }
        public double Price { get; set; }
        public  string? Thumbnail { get; set; }
        public string? Description { get; set; }
        public string? PreviewVideo { get; set; }
        public string? Status { get; set; }
        public int CategoryId { get; set; }
        public int Lessons { get; set; }
        public float Durations { get; set; }
        public int Enrollments { get; set; }
    }
}
