using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
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
        public ActionResult Thongbao()
        {
            if (Session["userid"] == null)
                return RedirectToAction("DangNhap", "Account");

            string userId = Session["userid"].ToString();

            // Lấy thông tin người dùng
            var user = data.USER_DATAs.FirstOrDefault(u => u.ID_USER == userId);

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("DangNhap", "Account");
            }

            return View(user); // Truyền model sang view
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
    }
}