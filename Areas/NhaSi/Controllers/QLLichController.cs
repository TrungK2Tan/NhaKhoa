using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaKhoa.Areas.NhaSi.Controllers
{
    public class QLLichController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();
        // GET: NhaSi/QLLich
        public ActionResult Index()
        {

            // Get the currently logged-in user's ID
            string currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            ViewBag.TenNhaSi = user.FullName;
            ViewBag.HinhAnh = user.HinhAnh;
            // Retrieve the ThoiKhoaBieu for the logged-in dentist
            var thoikhoabieus = db.ThoiKhoaBieux
                                 .Where(tkb => tkb.Id_Nhasi == currentUserId)
                                 .ToList();
            return View(thoikhoabieus);
        }
        public ActionResult ViewCalendar()
        {
            // Get the currently logged-in user's ID
            string currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            ViewBag.TenNhaSi = user.FullName;
            var thoikhoabieus = db.ThoiKhoaBieux.ToList();
            return View(thoikhoabieus);
        }
    }
}