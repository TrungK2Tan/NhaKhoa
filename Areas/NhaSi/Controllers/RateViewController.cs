using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NhaKhoa.Models;

namespace NhaKhoa.Areas.NhaSi.Controllers
{
    public class RateViewController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: NhaSi/RateView
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

                // Lấy đánh giá dựa trên ID của nha sĩ (Id_Nhasi)
                var danhGiaBinhLuans = db.DanhGias.Where(d => d.Id_Nhasi == userId).ToList();

                    return View(danhGiaBinhLuans);
                }
                return View();

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
