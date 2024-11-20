using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace E_Learning_Course.ViewModels
{
    public class CourseForCreation
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tiêu đề không được vượt quá 100 ký tự")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn một danh mục hợp lệ")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải là một số dương")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ít nhất một hình ảnh")]
        [DataType(DataType.Upload)]
        [Display(Name = "Chọn một hình ảnh để tải lên")]
        public IFormFile? Thumbnail { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        // Bỏ chú thích nếu bạn muốn thêm xác thực thời lượng
        /*
        [Required(ErrorMessage = "Thời lượng là bắt buộc")]
        [Range(typeof(TimeSpan), "00:00:01", "24:00:00", ErrorMessage = "Thời lượng phải từ 1 giây đến 24 giờ")]
        public TimeSpan? Duration { get; set; }
        */

        [Required(ErrorMessage = "Vui lòng chọn ít nhất một video")]
        [DataType(DataType.Upload)]
        [Display(Name = "Chọn một video để tải lên")]
        public IFormFile? PreviewVideo { get; set; }

        [Required(ErrorMessage = "Số ngày giới hạn là bắt buộc")]
        [Range(1, 365, ErrorMessage = "Số ngày giới hạn phải từ 1 đến 365 ngày")]
        public int? LimitDay { get; set; }
    }
}
