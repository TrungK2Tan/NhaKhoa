
using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaKhoa.Areas.NhaSi.Controllers
{
    public class NhaSiController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();
        // GET: NhaSi/NhaSi
        public ActionResult Index()
        {
                // Lấy thông tin người dùng đã đăng nhập
                var userId = User.Identity.GetUserId();
                var user = db.AspNetUsers.Find(userId);

                if (user != null)
                {
                    // Đã lấy được thông tin người dùng, bạn có thể sử dụng thông tin này
                    var userName = user.UserName;
                    var email = user.Email;
                ViewBag.TenNhaSi = user.FullName;
                ViewBag.HinhAnh = user.HinhAnh;
                // Thêm các thông tin khác về nha sĩ
            }
                return View();
        }
    }
}