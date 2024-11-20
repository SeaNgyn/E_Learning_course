using System;
using System.ComponentModel.DataAnnotations;

namespace E_Learning_Course.ViewModels
{
    public class UserForRegistration
    {
        [Required(ErrorMessage = "Không được để trống tên tài khoản")]
        [StringLength(50, ErrorMessage = "Tên người dùng không vượt quá 50 kí tự")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Hãy nhập mật khẩu")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Mật khẩu phải chưa ít nhất {2} kí tự")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Mật khẩu phải chứa ít nhất một chữ cái viết hoa, một chữ số và một ký tự đặc biệt")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Hãy nhập email")]
        [EmailAddress(ErrorMessage = "Sai định dạng email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui chọn ngày tháng năm sinh")]
        [DataType(DataType.Date, ErrorMessage = "Sai định dạng ngày tháng năm")]
        [CustomValidation(typeof(UserForRegistration), nameof(ValidateDateOfBirth))]
        public DateTime? DateOfBirth { get; set; }

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
    }
}
