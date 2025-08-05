using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DU_AN_WEB.Models
{
	public class RegisterModel
	{
        [Required(ErrorMessage = "Tên người dùng không được để trống")]
        public string NAME_USER { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        public string USER_PASSWORD { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("USER_PASSWORD", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        public string CONFIRM_PASSWORD { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string EMAIL { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string USER_ADDRESS { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã xác nhận")]
        public string VERIFICATION_CODE { get; set; }
    }
}