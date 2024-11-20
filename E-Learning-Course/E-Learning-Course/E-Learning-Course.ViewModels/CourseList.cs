using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class CourseList
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Category { get; set; }
        public string? Thumbnail { get; set; }
        public double Price { get; set; }
        public int Enrollments { get; set; }
        public int Lessons { get; set; }
        public float Durations { get; set; }

    }
}
