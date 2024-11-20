using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class CommentList
    {
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public int? ParentCommentId { get; set; } // Cho phép giá trị NULL
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Avatar { get; set; }
        public DateTime? UpdatedAt { get; set; } // Cho phép giá trị NULL nếu UpdatedAt có thể null
        public bool IsDelete { get; set; }
        public int? CourseId { get;set; }
    }
}
