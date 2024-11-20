using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class ChapterDTO
    {
        public int? Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public double? Duration { get; set; }
        public List<LessonDTO>? Lessons { get; set; }
    }
}
