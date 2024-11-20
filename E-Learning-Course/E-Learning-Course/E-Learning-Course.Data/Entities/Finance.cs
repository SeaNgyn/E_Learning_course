using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Data.Entities
{
    public class Finance
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public double Revenue { get; set; }
        public double Fee { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set;}

    }
}
