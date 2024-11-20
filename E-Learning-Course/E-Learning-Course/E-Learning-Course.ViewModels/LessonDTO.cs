using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class LessonDTO
    {
        public int Id { get; set; }
        [Display(Name = "Tiêu đề bài học")]
        [Required(ErrorMessage = "Tiêu đề bài học là bắt buộc.")]
        public string Name { get; set; }
        public int ChapterId { get; set; }

        public string? Type { get; set; }

        [Url(ErrorMessage = "Địa chỉ video không hợp lệ.")]
        public string? VideoUrl { get; set; }

        public string? Status { get; set; }

        [Display(Name = "Mô tả bài học")]
        [Required(ErrorMessage = "Mô tả bài học là bắt buộc.")]
        public string? Content { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Thời lượng phải là một số dương.")]
        public double? Duration { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Điểm qua bài phải là số dương")]
        public double? Passing { get; set; }

        public string? StatusProgress { get; set; }
        public bool? IsPass { get; set; }
        public List<QuestionDTO>? Questions { get; set; }
    }
}
