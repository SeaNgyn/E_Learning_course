using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.ViewModels
{
    public class UserEditDTO
    {
        [Required(ErrorMessage = "Không được để trống tên người dùng")]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        //[Required(ErrorMessage = "Username is required")]
        //[StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        //public string? UserName { get; set; }

        [Required(ErrorMessage = "Số điện thoại bắt buộc")]
        //[RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        //[EmailAddress(ErrorMessage = "Invalid email address")]
        //public string? Email { get; set; }
    }
}
