using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class CourseTransaction
    {
        public int CourseId { get; set; }
        public string CustomerId { get; set; }
        public double Price { get; set; }
        public string Ip { get; set; }
        public bool ReturnBack { get; set; }
    }
}
