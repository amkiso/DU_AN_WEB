using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DU_AN_WEB.Models; // Thêm namespace chứa các model nếu cần
namespace DU_AN_WEB.Controllers
{
    
    public class HomeController : Controller
    {
        QL_SANPHAMDataContext data = new QL_SANPHAMDataContext(); // Khởi tạo context để truy cập cơ sở dữ liệu
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DangNhap()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitForm(FormCollection account)
        {
            string username = account["username"];
            string password = account["password"];

            // Kiểm tra nhập thiếu
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Message = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View("DangNhap");
            }

            // Kiểm tra thông tin người dùng trong CSDL
            USER_DATA user = data.USER_DATAs.FirstOrDefault(
                u => u.NAME_USER == username && u.USER_PASSWORD == password
            );

            if (user == null)
            {
                ViewBag.Message = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View("DangNhap");
            }

            // Đăng nhập thành công
            ViewBag.Message = "Đăng nhập thành công!";
            return RedirectToAction("Index", "Home"); // Hoặc chuyển hướng đến trang chính sau đăng nhập
        }

    }
}