﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NhaKhoa.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NhaKhoa.Areas.Admin.Controllers
{
    public class ManagerBookingController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: Admin/ManagerBooking
        public ActionResult Index()
        {
            var phieuDatLiches = db.PhieuDatLiches.Include(p => p.AspNetUser).Include(p => p.HinhThucThanhToan).Include(p => p.ThoiKhoaBieu);

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
            return View(phieuDatLiches.ToList());
        }

        

        public async Task<ActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PhieuDatLich lichHen = db.PhieuDatLiches.Find(id);
            if (lichHen == null)
            {
                return HttpNotFound();
            }

            lichHen.TrangThai = true; // Cập nhật thuộc tính IsApproved thành true
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
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
