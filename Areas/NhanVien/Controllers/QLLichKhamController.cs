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
using PagedList;

namespace NhaKhoa.Areas.NhanVien.Controllers
{
    public class QLLichKhamController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: NhanVien/QLLichKham
        public ActionResult Index(int? page)
        {
            // Lấy thông tin người dùng đã đăng nhập
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(userId);
            ViewBag.TenNhanVien = user.FullName;

            // Retrieve the PhieuDatLiches data and order it by NgayKham
            var phieuDatLiches = db.PhieuDatLiches
                .Include(p => p.AspNetUser)
                .Include(p => p.HinhThucThanhToan)
                .Include(p => p.ThoiKhoaBieu)
                .OrderBy(p => p.NgayKham);

            // Tạo một Dictionary để lưu trữ tên của NhaSi dựa trên IdNhaSi
            Dictionary<string, string> nhaSiNames = new Dictionary<string, string>();

            foreach (var lichHen in phieuDatLiches)
            {
                // Kiểm tra xem IdNhaSi đã được thêm vào Dictionary chưa
                if (!nhaSiNames.ContainsKey(lichHen.IdNhaSi))
                {
                    // Nếu chưa, thực hiện truy vấn và thêm vào Dictionary
                    var nhaSi = db.AspNetUsers.Find(lichHen.IdNhaSi);
                    if (nhaSi != null)
                    {
                        nhaSiNames.Add(lichHen.IdNhaSi, nhaSi.FullName);
                    }
                }
            }

            // Truyền danh sách lịch hẹn và tên của NhaSi vào View
            ViewBag.NhaSiNames = nhaSiNames;

            // Specify your desired page size
            int pageSize = 2;

            // Determine the current page number
            int pageNumber = (page ?? 1);

            // Use ToPagedList to get a subset of the list based on the current page and page size
            IPagedList<PhieuDatLich> pagedDatLich = phieuDatLiches.ToPagedList(pageNumber, pageSize);

            return View(pagedDatLich);
        }

        // GET: NhanVien/QLLichKham/Details/5
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
        // GET: NhanVien/QLLichKham/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: NhanVien/QLLichKham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PhieuDatLich phieuDatLich = db.PhieuDatLiches.Find(id);
            db.PhieuDatLiches.Remove(phieuDatLich);
            db.SaveChanges();
            return RedirectToAction("Index");
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
