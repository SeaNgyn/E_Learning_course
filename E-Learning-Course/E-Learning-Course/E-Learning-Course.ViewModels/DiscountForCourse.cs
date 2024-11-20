using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class DiscountForCourse
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "Code is required")]
        [StringLength(50, ErrorMessage = "Code cannot be longer than 50 characters")]
        public string? Code { get; set; }

        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100")]
        public decimal DiscountPer { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max uses must be at least 1")]
        public int MaxUses { get; set; }

        //[Required(ErrorMessage = "Start date is required")]
        public DateTime? StartDate { get; set; }

        //[Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [FutureDate(ErrorMessage = "End date must be after the start date")]
        public DateTime? EndDate { get; set; }


        public int CourseName { get; set; }
    }

    // Custom validation attribute for checking if the end date is after the start date
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (DiscountForCourse)validationContext.ObjectInstance;
            if (model.EndDate <= model.StartDate)
            {
                return new ValidationResult(ErrorMessage ?? "End date must be after start date.");
            }
            return ValidationResult.Success;
        }
    }

}
