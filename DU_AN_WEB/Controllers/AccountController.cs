using DU_AN_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DU_AN_WEB.Controllers
{
    public class AccountController : Controller
    {
        QL_SANPHAMDataContext data = new QL_SANPHAMDataContext(); // Khởi tạo context để truy cập cơ sở dữ liệu
        // GET: Account
        public ActionResult DangKi()
        {
            return View();
        }
        
        public ActionResult DangNhap()
        {
            return View();
        }
        public ActionResult DangXuat()
        {
            // Xóa thông tin người dùng khỏi Session
            Session.Clear();
            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("DangNhap", "Account");
        }
        public ActionResult QuanLy() { return View(); }
        public ActionResult User() { return View(); }
        public ActionResult UserInfo()
        {
            // Kiểm tra xem session có tồn tại không
            if (Session["userid"] == null)
            {
                // Nếu chưa đăng nhập thì chuyển về trang đăng nhập
                return RedirectToAction("DangNhap", "Account");
            }

            // Lấy ID người dùng từ session
            string userId = Session["userid"].ToString();

            // Truy vấn thông tin người dùng từ CSDL
            var user = data.USER_DATAs.FirstOrDefault(u => u.ID_USER == userId);

            if (user == null)
            {
                // Nếu không tìm thấy user trong DB thì xóa session và về đăng nhập
                Session.Clear();
                return RedirectToAction("DangNhap", "Account");
            }

            // Kiểm tra quyền của người dùng
            if (user.USER_TYPE == "ADMINISTRATOR")
            {
                return RedirectToAction("QuanLy", "Account");
            }
            else
            {
                return RedirectToAction("User", "Account");
            }
        }


        [HttpPost]
        public ActionResult SubmitForm(FormCollection account)
        {
            string username = account["username"];
            string password = account["password"];
            // --- BẮT ĐẦU CODE DEBUG ---
            // Lưu các giá trị nhận được từ form vào ViewBag để hiển thị ra View
            ViewBag.DebugUsername = username;
            ViewBag.DebugPassword = password; // CẢNH BÁO: Chỉ hiển thị mật khẩu khi debug!
            ViewBag.DebugRole = "Chưa tìm thấy user trong CSDL"; // Gán giá trị mặc định
                                                                 // --- KẾT THÚC CODE DEBUG ---
                                                                 // Kiểm tra nhập thiếu
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
                return View("DangNhap");
            }

            // Kiểm tra thông tin người dùng trong CSDL
            USER_DATA user = data.USER_DATAs.FirstOrDefault(
                u => u.NAME_USER == username && u.USER_PASSWORD == password
            );
            
            if (user == null)
            {
                ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View("DangNhap");
            }

            // ✅ Lưu thông tin người dùng vào Session
            Session["username"] = user.NAME_USER;
            Session["userid"] = user.ID_USER;  // nếu có ID
            Session["role"] = user.USER_TYPE;        // nếu có vai trò
            ViewBag.message = "Đăng nhập thành công!";
            
            // Đăng nhập thành công
            return RedirectToAction("ThongBao", "Home");
        }


        public JsonResult SendOtpCode(Otpmodel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return Json(new { success = false, message = "Email không hợp lệ." });

            var otp = new Random().Next(100000, 999999).ToString(); // mã 6 số
            model.Code = otp;
            model.ExpirationTime = DateTime.Now.AddMinutes(5);

            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("thangcuoi1984a@gmail.com", "taoy fbrf cpuo rswr"),
                    EnableSsl = true,
                };

                var mail = new MailMessage
                {
                    From = new MailAddress("your_email@gmail.com", "Tên website"),
                    Subject = "Mã xác nhận đăng ký",
                    Body = $"Mã xác nhận của bạn là: {otp}",
                    IsBodyHtml = false,
                };
                mail.To.Add(model.Email);

                smtpClient.Send(mail);
                return Json(new { success = true, message = "Mã xác nhận đã được gửi đến " + model.Email });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi gửi email: " + ex.Message });
            }
        }

    }
}