using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace E_Learning_Course.ViewModels
{
    public class CourseForUpdate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public double Price { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Choose an image to upload")]
        public IFormFile? Thumbnail { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description can't be longer than 1000 characters")]
        public string? Description { get; set; }

        // Uncomment this if you want to add duration validation
        /*
        [Required(ErrorMessage = "Duration is required")]
        [Range(typeof(TimeSpan), "00:00:01", "24:00:00", ErrorMessage = "Duration must be between 1 second and 24 hours")]
        public TimeSpan? Duration { get; set; }
        */

        [Required(ErrorMessage = "Please choose at least one video")]
        [DataType(DataType.Upload)]
        [Display(Name = "Choose a video to upload")]
        public IFormFile? PreviewVideo { get; set; }

        // Optional: LimitDay property with validation
        [Range(1, 365, ErrorMessage = "Limit Day must be between 1 and 365 days")]
        public int? LimitDay { get; set; }
    }
}
