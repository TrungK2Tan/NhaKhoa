using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using NhaKhoa.Models;
using PagedList;

namespace NhaKhoa.Areas.Admin.Controllers
{
    public class DoanhThuLichKhamController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: Admin/DoanhThuLichKham
        public ActionResult Index(DateTime? selectedDate, string filterType)
        {
            var phieuDatLich = db.PhieuDatLiches.Include(p => p.AspNetUser).Include(p => p.HinhThucThanhToan).Include(p => p.ThoiKhoaBieu);

            if (selectedDate.HasValue)
            {
                switch (filterType)
                {
                    case "day":
                        // Filter by the selected day
                        phieuDatLich = phieuDatLich.Where(p => DbFunctions.TruncateTime(p.NgayKham) == DbFunctions.TruncateTime(selectedDate.Value));
                        break;
                    case "week":
                        // Filter by the current week
                        var startOfWeek = selectedDate.Value.Date.AddDays(-(int)selectedDate.Value.DayOfWeek);
                        var endOfWeek = startOfWeek.AddDays(6);
                        phieuDatLich = phieuDatLich.Where(p => p.NgayKham >= startOfWeek && p.NgayKham <= endOfWeek);
                        break;
                    case "month":
                        // Filter by the current month
                        phieuDatLich = phieuDatLich.Where(p => p.NgayKham.Value.Year == selectedDate.Value.Year && p.NgayKham.Value.Month == selectedDate.Value.Month);
                        break;
                    default:
                        break;
                }
            }

            // Sort by date
            phieuDatLich = phieuDatLich.OrderBy(p => p.NgayKham);

            var idNhaSiList = phieuDatLich.Select(p => p.IdNhaSi).ToList();
            var idBenhNhaList = phieuDatLich.Select(p => p.IdBenhNhan).ToList();

            var nhaSiDict = db.AspNetUsers
                .Where(u => idNhaSiList.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.FullName);

            var benhNhanDict = db.AspNetUsers
                .Where(u => idBenhNhaList.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u.FullName);

            // Pass information to ViewBag
            ViewBag.FullNameNhaSi = nhaSiDict;
            ViewBag.FullNameBenhNhan = benhNhanDict;

            // Calculate and format the total revenue
            double? DoanhThu = phieuDatLich.Sum(p => p.Gia ?? 0);
            string formattedDoanhThu = string.Format(new CultureInfo("vi-VN"), "{0:C0}", DoanhThu);
            ViewBag.DoanhThu = formattedDoanhThu;

            return View(phieuDatLich.ToList());
        }

        // GET: Admin/DoanhThuLichKham/Details/5
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

        // GET: Admin/DoanhThuLichKham/Create
        public ActionResult Create()
        {
            ViewBag.IdBenhNhan = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Id_hinhthuc = new SelectList(db.HinhThucThanhToans, "Id_hinhthuc", "TenHinhThuc");
            ViewBag.Id_TKB = new SelectList(db.ThoiKhoaBieux, "Id_TKB", "Id_Nhasi");
            return View();
        }

        // POST: Admin/DoanhThuLichKham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Phieudat,NgayKham,Gia,Id_hinhthuc,IdNhaSi,IdBenhNhan,Id_TKB,STT,TrangThai,TrangThaiThanhToan")] PhieuDatLich phieuDatLich)
        {
            if (ModelState.IsValid)
            {
                db.PhieuDatLiches.Add(phieuDatLich);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBenhNhan = new SelectList(db.AspNetUsers, "Id", "Email", phieuDatLich.IdBenhNhan);
            ViewBag.Id_hinhthuc = new SelectList(db.HinhThucThanhToans, "Id_hinhthuc", "TenHinhThuc", phieuDatLich.Id_hinhthuc);
            ViewBag.Id_TKB = new SelectList(db.ThoiKhoaBieux, "Id_TKB", "Id_Nhasi", phieuDatLich.Id_TKB);
            return View(phieuDatLich);
        }

        // GET: Admin/DoanhThuLichKham/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.IdBenhNhan = new SelectList(db.AspNetUsers, "Id", "Email", phieuDatLich.IdBenhNhan);
            ViewBag.Id_hinhthuc = new SelectList(db.HinhThucThanhToans, "Id_hinhthuc", "TenHinhThuc", phieuDatLich.Id_hinhthuc);
            ViewBag.Id_TKB = new SelectList(db.ThoiKhoaBieux, "Id_TKB", "Id_Nhasi", phieuDatLich.Id_TKB);
            return View(phieuDatLich);
        }

        // POST: Admin/DoanhThuLichKham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Phieudat,NgayKham,Gia,Id_hinhthuc,IdNhaSi,IdBenhNhan,Id_TKB,STT,TrangThai,TrangThaiThanhToan")] PhieuDatLich phieuDatLich)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phieuDatLich).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBenhNhan = new SelectList(db.AspNetUsers, "Id", "Email", phieuDatLich.IdBenhNhan);
            ViewBag.Id_hinhthuc = new SelectList(db.HinhThucThanhToans, "Id_hinhthuc", "TenHinhThuc", phieuDatLich.Id_hinhthuc);
            ViewBag.Id_TKB = new SelectList(db.ThoiKhoaBieux, "Id_TKB", "Id_Nhasi", phieuDatLich.Id_TKB);
            return View(phieuDatLich);
        }

        // GET: Admin/DoanhThuLichKham/Delete/5
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

        // POST: Admin/DoanhThuLichKham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PhieuDatLich phieuDatLich = db.PhieuDatLiches.Find(id);
            db.PhieuDatLiches.Remove(phieuDatLich);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ExportToExcel()
        {
            var phieudat = db.PhieuDatLiches.ToList(); // Lấy tất cả các dữ liệu từ cơ sở dữ liệu

            if (phieudat == null || phieudat.Count == 0)
            {
                return HttpNotFound();
            }

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("DoanhThuLichKham");

            // Thiết lập dữ liệu trong file Excel
            ws.Cell(1, 1).Value = "ID Phiếu Đặt";
            ws.Cell(1, 2).Value = "ID Bệnh Nhân";
            ws.Cell(1, 3).Value = "ID Nha Sĩ";
            ws.Cell(1, 4).Value = "Ngày khám";
            ws.Cell(1, 5).Value = "Số thứ tự";
            ws.Cell(1, 6).Value = "Giá";
            ws.Cell(1, 7).Value = "Hình Thức Thanh Toán";
            ws.Cell(1, 8).Value = "Tổng Doanh Thu";

            int row = 2;
            decimal totalDoanhThu = 0; // Tổng doanh thu

            foreach (var thuoc in phieudat)
            {
                ws.Cell(row, 1).Value = thuoc.Id_Phieudat;
                ws.Cell(row, 2).Value = thuoc.IdBenhNhan;
                ws.Cell(row, 3).Value = thuoc.IdNhaSi;
                ws.Cell(row, 4).Value = thuoc.NgayKham;
                ws.Cell(row, 5).Value = thuoc.STT;
                ws.Cell(row, 6).Value = thuoc.Gia ?? 0; // Use the null coalescing operator to handle nullable double
                ws.Cell(row, 7).Value = thuoc.HinhThucThanhToan.Id_hinhthuc;

                totalDoanhThu += Convert.ToDecimal(thuoc.Gia.GetValueOrDefault()); // Convert to decimal

                row++;
            }

            // Gán giá trị tổng doanh thu vào ô tương ứng
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
            var fileName = "DoanhThuLichKham.xlsx";

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