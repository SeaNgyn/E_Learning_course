using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        // Foreign keys
        public int LessonId { get; set; }
        public string? UserId { get; set; }

        // Khóa ngoại tự tham chiếu (có thể null)
        public int? ParentCommentId { get; set; }

        public string? Content { get; set; }

        // Thời gian tạo và cập nhật
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDelete { get; set; }

        // Navigation properties
        public Lesson ?Lesson { get; set; }
        public User ?User { get; set; }

        // Navigation property cho bình luận cha
        public Comment? ParentComment { get; set; }

        // Navigation property cho các bình luận con (nếu có)
        public ICollection<Comment>? ChildComments { get; set; }
    }
}
