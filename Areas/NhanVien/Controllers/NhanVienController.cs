using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaKhoa.Areas.NhanVien.Controllers
{
    public class NhanVienController : Controller
    {
        // GET: NhanVien/NhanVien
        private NhaKhoaModel db = new NhaKhoaModel();
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
                ViewBag.TenNhanVien = user.FullName;
                // Thêm các thông tin khác về nha sĩ
            }
            return View();
        }
    }
}