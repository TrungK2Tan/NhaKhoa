using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using NhaKhoa.Models;

namespace NhaKhoa.Areas.Admin.Controllers
{
    public class DoanhThuBanThuocController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: Admin/DoanhThuBanThuoc
        public ActionResult Index()
        {
            var donThuocs = db.DonThuocs.Include(d => d.PhieuDatLich);
            return View(donThuocs.ToList());
        }

        // GET: Admin/DoanhThuBanThuoc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonThuoc donThuoc = db.DonThuocs.Find(id);
            if (donThuoc == null)
            {
                return HttpNotFound();
            }
            return View(donThuoc);
        }

        // GET: Admin/DoanhThuBanThuoc/Create
        public ActionResult Create()
        {
            ViewBag.Id_phieudat = new SelectList(db.PhieuDatLiches, "Id_Phieudat", "IdNhaSi");
            return View();
        }

        // POST: Admin/DoanhThuBanThuoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_donthuoc,Id_phieudat,Chandoan,NgayGio,Soluong,TongTien")] DonThuoc donThuoc)
        {
            if (ModelState.IsValid)
            {
                db.DonThuocs.Add(donThuoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_phieudat = new SelectList(db.PhieuDatLiches, "Id_Phieudat", "IdNhaSi", donThuoc.Id_phieudat);
            return View(donThuoc);
        }

        // GET: Admin/DoanhThuBanThuoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonThuoc donThuoc = db.DonThuocs.Find(id);
            if (donThuoc == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_phieudat = new SelectList(db.PhieuDatLiches, "Id_Phieudat", "IdNhaSi", donThuoc.Id_phieudat);
            return View(donThuoc);
        }

        // POST: Admin/DoanhThuBanThuoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_donthuoc,Id_phieudat,Chandoan,NgayGio,Soluong,TongTien")] DonThuoc donThuoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donThuoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_phieudat = new SelectList(db.PhieuDatLiches, "Id_Phieudat", "IdNhaSi", donThuoc.Id_phieudat);
            return View(donThuoc);
        }

        // GET: Admin/DoanhThuBanThuoc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonThuoc donThuoc = db.DonThuocs.Find(id);
            if (donThuoc == null)
            {
                return HttpNotFound();
            }
            return View(donThuoc);
        }

        // POST: Admin/DoanhThuBanThuoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DonThuoc donThuoc = db.DonThuocs.Find(id);
            db.DonThuocs.Remove(donThuoc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ExportToExcel()
        {
            var donthuoc = db.DonThuocs.ToList(); // Lấy tất cả các dữ liệu từ cơ sở dữ liệu

            if (donthuoc == null || donthuoc.Count == 0)
            {
                return HttpNotFound();
            }

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("DoanThuBanThuoc");

            // Thiết lập dữ liệu trong file Excel
            ws.Cell(1, 1).Value = "ID Đơn thuốc";
            ws.Cell(1, 2).Value = "ID Phiếu Đặt";
            ws.Cell(1, 3).Value = "Chẩn đoán";
            ws.Cell(1, 4).Value = "Số lượng thuốc";
            ws.Cell(1, 5).Value = "Tổng tiền";
            ws.Cell(1, 6).Value = "Ngày giờ lập đơn";
            ws.Cell(1, 7).Value = "Tổng Doanh Thu";

            int row = 2;
            decimal totalDoanhThu = 0; // Tổng doanh thu
            foreach (var thuoc in donthuoc)
            {
                ws.Cell(row, 1).Value = thuoc.Id_donthuoc;
                ws.Cell(row, 2).Value = thuoc.Id_phieudat;
                ws.Cell(row, 3).Value = thuoc.Chandoan;
                ws.Cell(row, 4).Value = thuoc.Soluong;
                ws.Cell(row, 5).Value = thuoc.TongTien ?? 0;
                ws.Cell(row, 6).Value = thuoc.NgayGio;
                totalDoanhThu += Convert.ToDecimal(thuoc.TongTien.GetValueOrDefault()); // Convert to decimal
                row++;
            }
            ws.Cell(2, 8).Value = totalDoanhThu;
            // Lưu file Excel
            // Lưu file Excel vào một mảng byte
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Trả về file Excel đã tạo
            var fileName = "DoanhThuBanThuoc.xlsx";

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.BinaryWrite(fileBytes);
            Response.End();

            return null; // Tránh thêm HTML vào response
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
