using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DU_AN_WEB.Models
{
    public class UserModel
    {
        [Required]
        [StringLength(10)]
        public string ID_USER { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Tên tài khoản chỉ được chứa chữ và số, không dấu, không cách")]
        [Display(Name = "Tên tài khoản")]
        [StringLength(20)]
        public string NAME_USER { get; set; }

        [Required]
        [StringLength(64)]
        [DataType(DataType.Password)]
        [StrongPassword]
        public string USER_PASSWORD { get; set; }

        public DateTime DAY_CREATED { get; set; } = DateTime.Now;

        [StringLength(50)]
        [Display(Name = "Địa chỉ")]
        public string USER_ADDRESS { get; set; }

        [Display(Name = "Loại tài khoản")]
        public string USER_TYPE { get; set; } = "User";

        public string USER_STATUS { get; set; } = "Active";

        [Required]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(50)]
        public string EMAIL { get; set; }
    }

    // Custom Attribute để kiểm tra mật khẩu mạnh
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            string password = Convert.ToString(value);
            if (string.IsNullOrWhiteSpace(password)) return false;

            // Mật khẩu phải có ít nhất 8 ký tự, gồm: chữ hoa, chữ thường, số và ký tự đặc biệt
            bool isValid = password.Length >= 8 &&
                           Regex.IsMatch(password, "[a-z]") &&
                           Regex.IsMatch(password, "[A-Z]") &&
                           Regex.IsMatch(password, "[0-9]") &&
                           Regex.IsMatch(password, @"[\W_]");

            return isValid;
        }

        public override string FormatErrorMessage(string name)
        {
            return "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt.";
        }
    }
}