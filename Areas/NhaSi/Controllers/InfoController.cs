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
    public class InfoController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: NhaSi/Info
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
                // Thêm các thông tin khác về nha sĩ
            }

            return View();
        }
        [HttpGet]
        public ActionResult Edit()
        {
            string userId = User.Identity.GetUserId();
            if (userId != null)
            {
                AspNetUser nhaSi = db.AspNetUsers.Find(userId);
                if (nhaSi != null)
                {
                    return View(nhaSi);
                }
            }
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AspNetUser nhaSi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhaSi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nhaSi);
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
