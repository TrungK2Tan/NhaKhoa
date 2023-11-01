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
            // Lấy ID của nha sĩ đã đăng nhập
            string userId = User.Identity.GetUserId();

            if (userId != null)
            {
                // Lấy thông tin nha sĩ từ cơ sở dữ liệu bằng ID
                AspNetUser nhaSi = db.AspNetUsers.Find(userId);

                if (nhaSi != null)
                {
                    return View(nhaSi);
                }
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
