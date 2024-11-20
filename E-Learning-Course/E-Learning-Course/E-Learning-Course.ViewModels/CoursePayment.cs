using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class CoursePayment
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Thumbnail {  get; set; }
        public double Price { get; set; }
        public string Url { get; set; }
        public int Lessons { get; set; }
        public double Durations { get; set; }

    }
}
