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
    public class QLLKhamController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: NhaSi/QLLKham
        public ActionResult Index(string searchString, string filterDate)
        {
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(userId);
            ViewBag.TenNhaSi = user.FullName;
            ViewBag.HinhAnh = user.HinhAnh;
            ViewBag.CurrentUserId = userId;

            // If filterDate is null or empty, set it to the current date
            if (string.IsNullOrEmpty(filterDate))
            {
                filterDate = DateTime.Now.ToString("yyyy-MM-dd");
            }

            // Parse the filterDate to DateTime
            if (DateTime.TryParse(filterDate, out var selectedDate))
            {
                // Sắp xếp danh sách lịch khám theo NgayKham và STT
                IQueryable<PhieuDatLich> phieuDatLiches = db.PhieuDatLiches
                    .Where(p => p.IdNhaSi == userId)
                    .Include(p => p.AspNetUser)
                    .Include(p => p.HinhThucThanhToan)
                    .Include(p => p.ThoiKhoaBieu)
                    .Where(p => DbFunctions.TruncateTime(p.NgayKham) == selectedDate.Date)
                    .OrderBy(p => p.NgayKham)
                    .ThenBy(p => p.STT);

                return View(phieuDatLiches.ToList());
            }
            else
            {
                // Handle invalid date format
                ModelState.AddModelError("filterDate", "Invalid date format");
                return View(new List<PhieuDatLich>());
            }
        }


        public ActionResult ViewAllBooking(string searchString, string filterDate)
        {
            // Lấy thông tin người dùng đã đăng nhập
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(userId);
            ViewBag.TenNhaSi = user.FullName;
            ViewBag.HinhAnh = user.HinhAnh;
            ViewBag.CurrentUserId = userId;

            // Sắp xếp danh sách lịch khám theo NgayKham và STT
            IQueryable<PhieuDatLich> phieuDatLiches = db.PhieuDatLiches
                .Where(p => p.IdNhaSi == userId)
                .Include(p => p.AspNetUser)
                .Include(p => p.HinhThucThanhToan)
                .Include(p => p.ThoiKhoaBieu);

            // Thêm điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                phieuDatLiches = phieuDatLiches
                    .Where(p => p.AspNetUser.Email.Contains(searchString) || p.AspNetUser.FullName.Contains(searchString));
            }

            // Thêm điều kiện lọc theo ngày
            if (!string.IsNullOrEmpty(filterDate) && DateTime.TryParse(filterDate, out var selectedDate))
            {
                phieuDatLiches = phieuDatLiches
                    .Where(p => DbFunctions.TruncateTime(p.NgayKham) == selectedDate.Date);
            }

            // Sắp xếp danh sách sau khi thêm điều kiện tìm kiếm và lọc ngày
            phieuDatLiches = phieuDatLiches
                .OrderBy(p => p.NgayKham)
                .ThenBy(p => p.STT);

            return View(phieuDatLiches.ToList());
        }

        // GET: NhaSi/QLLKham/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhieuDatLich phieuDatLich = db.PhieuDatLiches.Find(id);
            if (phieuDatLich == null)
            {
                return HttpNotFound();
            }
            return View(phieuDatLich);
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
