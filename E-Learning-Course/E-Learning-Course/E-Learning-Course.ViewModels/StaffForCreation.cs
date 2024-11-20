using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Learning_Course.ViewModels
{
    public class StaffForCreation
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        [CustomValidation(typeof(StaffForCreation), nameof(ValidateDateOfBirth))]
        public DateTime DateOfBirth { get; set; }

        // Custom validation method for DateOfBirth
        public static ValidationResult? ValidateDateOfBirth(DateTime dateOfBirth, ValidationContext context)
        {
            if (dateOfBirth > DateTime.Now)
            {
                return new ValidationResult("Date of birth cannot be in the future");
            }
            if (dateOfBirth < DateTime.Now.AddYears(-120)) // Assuming 120 years as the max age
            {
                return new ValidationResult("Date of birth is too far in the past");
            }
            return ValidationResult.Success;
        }

        [Required(ErrorMessage = "Status is required")]
        [Range(0, 2, ErrorMessage = "Status must be between 0 and 2 (0: Inactive, 1: Active, 2: Suspended)")]
        public int Status { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [StringLength(50, ErrorMessage = "Role cannot be longer than 50 characters")]
        public string SelectedRole { get; set; }

        
        [DataType(DataType.Upload)]
        public IFormFile? Avatar { get; set; }
        public string? AvatarUrl { get; set; }

        // Additional validation for the Avatar file
        public static ValidationResult? ValidateAvatar(IFormFile avatar, ValidationContext context)
        {
            if (avatar == null || avatar.Length == 0)
            {
                return new ValidationResult("Avatar file is required");
            }

            // You can add more checks, such as file type and size
            string[] validExtensions = { ".jpg", ".jpeg", ".png" };
            var fileExtension = System.IO.Path.GetExtension(avatar.FileName).ToLowerInvariant();

            if (!validExtensions.Contains(fileExtension))
            {
                return new ValidationResult("Invalid file type. Only .jpg, .jpeg, and .png files are allowed.");
            }

            if (avatar.Length > 5 * 1024 * 1024) // Limit to 5 MB
            {
                return new ValidationResult("File size must not exceed 5 MB");
            }

            return ValidationResult.Success;
        }
    }
}
