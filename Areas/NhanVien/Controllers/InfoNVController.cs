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

namespace NhaKhoa.Areas.NhanVien.Controllers
{
    public class InfoNVController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: NhanVien/InfoNV
        public ActionResult Index()
        {
            // Lấy thông tin người dùng đã đăng nhập
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(userId);

            if (user != null)
            {
                // Lấy thông tin người dùng thành công, bạn có thể sử dụng nó trong view
                ViewBag.TenNhanVien = user.FullName;
                ViewBag.UserName = user.UserName;
                ViewBag.Email = user.Email;
                return View(user);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit()
        {
            string userId = User.Identity.GetUserId();
            if (userId != null)
            {
                AspNetUser nhanvien = db.AspNetUsers.Find(userId);
                if (nhanvien != null)
                {
                    return View(nhanvien);
                }
            }
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AspNetUser nhanvien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhanvien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nhanvien);
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
