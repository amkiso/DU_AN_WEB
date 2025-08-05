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
        public ActionResult QuenMatKhau()
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
            if (user.USER_TYPE =="QuanLy") {
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

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View("Dangki", model);

            // Kiểm tra email đã tồn tại
            if (data.USER_DATAs.Any(u => u.EMAIL.Trim() == model.EMAIL.Trim()))
            {
                ViewBag.Message = "Email đã được sử dụng.!";
                return View("Dangki", model);
            }

            // ✅ Lấy OTP từ session theo email
            var otpData = Session["OTP_" + model.EMAIL] as Otpmodel;

            if (otpData == null)
            {
                ViewBag.Message = "Bạn chưa yêu cầu mã xác nhận.";
                return View("Dangki", model);
            }

            if (DateTime.Now > otpData.ExpirationTime)
            {
                ViewBag.Message = "Mã xác nhận đã hết hạn.";
                return View("Dangki", model);
            }

            if (model.VERIFICATION_CODE != otpData.Code)
            {
                ViewBag.Message = "Mã xác nhận không đúng.";
                return View("Dangki", model);
            }

            // Tạo ID_USER duy nhất
            string idUser;
            do
            {
                idUser = "USER" + new Random().Next(100000, 999999).ToString();
            } while (data.USER_DATAs.Any(u => u.ID_USER == idUser));

            USER_DATA newUser = new USER_DATA
            {
                ID_USER = idUser,
                NAME_USER = model.NAME_USER,
                USER_PASSWORD = model.USER_PASSWORD, // hoặc mã hóa nếu cần
                USER_ADDRESS = model.USER_ADDRESS,
                EMAIL = model.EMAIL,
                DAY_CREATED = DateTime.Now,
                USER_TYPE = "Customer",
                USER_STATUS = "Active",
                PROFILE_PICTURE = null
            };

            data.USER_DATAs.InsertOnSubmit(newUser);
            data.SubmitChanges();

            TempData["SuccessMessage"] = "Đăng ký thành công! Mời bạn đăng nhập.";
            return RedirectToAction("Dangnhap", "Account");
        }

        private string GenerateUserId()
        {
            Random rnd = new Random();
            return "USER" + rnd.Next(100000, 999999).ToString();
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
                    Subject = "Mã xác nhận đăng ký tài khoản mua hàng",
                    Body = $"Xin chào {model.Email} !" +
                    $"\nMã xác nhận của bạn là: {otp}"+
                    $"\nĐây là mã xác nhận để tạo tài khoản mua hàng tại MY_PROJECT.COM"+
                    $"\nMã này sẽ hết hạn sau 5 phút." +
                    $"\nVui lòng không chia sẻ mã này với bất kỳ ai."+
                    $"\nNếu bạn không phải là người yêu cầu mã này, vui lòng bỏ qua email này." +
                    $"\nCảm ơn bạn đã sử dụng dịch vụ của chúng tôi.!",
                    IsBodyHtml = false,
                };
                mail.To.Add(model.Email);

                smtpClient.Send(mail);

                // ✅ Lưu OTP vào session theo email
                Session["OTP_" + model.Email] = model;

                return Json(new { success = true, message = "Mã xác nhận đã được gửi đến " + model.Email });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi gửi email: " + ex.Message });
            }
        }


    }
}