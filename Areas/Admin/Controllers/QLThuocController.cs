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
using PagedList;

namespace NhaKhoa.Areas.Admin.Controllers
{
    public class QLThuocController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: Admin/QLThuoc
        public ActionResult Index(int? page)
        {
            int pageSize = 10; // Set the number of items per page here
            int pageNumber = (page ?? 1);
            var thuocList = db.Thuocs.ToList().Select(thuoc => new Thuoc
            {
                Id_thuoc = thuoc.Id_thuoc,
                Tenthuoc = thuoc.Tenthuoc,
                Mota = thuoc.Mota,
                Gia = thuoc.Gia,
                NgaySX = thuoc.NgaySX.HasValue ? thuoc.NgaySX.Value.Date : (DateTime?)null,
                HanSD = thuoc.HanSD.HasValue ? thuoc.HanSD.Value.Date : (DateTime?)null,
                Soluong = thuoc.Soluong,
                Thanhphan = thuoc.Thanhphan
            }).ToList();

            return View(thuocList.ToPagedList(pageNumber, pageSize));
        }

        // GET: Admin/QLThuoc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thuoc thuoc = db.Thuocs.Find(id);
            if (thuoc == null)
            {
                return HttpNotFound();
            }
            return View(thuoc);
        }

        // GET: Admin/QLThuoc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/QLThuoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_thuoc,Tenthuoc,Mota,Gia,NgaySX,HanSD,Soluong,Thanhphan")] Thuoc thuoc)
        {
            if (ModelState.IsValid)
            {
                db.Thuocs.Add(thuoc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thuoc);
        }

        // GET: Admin/QLThuoc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thuoc thuoc = db.Thuocs.Find(id);
            if (thuoc == null)
            {
                return HttpNotFound();
            }
            return View(thuoc);
        }

        // POST: Admin/QLThuoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_thuoc,Tenthuoc,Mota,Gia,NgaySX,HanSD,Soluong,Thanhphan")] Thuoc thuoc)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thuoc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuoc);
        }

        // GET: Admin/QLThuoc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thuoc thuoc = db.Thuocs.Find(id);
            if (thuoc == null)
            {
                return HttpNotFound();
            }
            return View(thuoc);
        }

        // POST: Admin/QLThuoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Thuoc thuoc = db.Thuocs.Find(id);
            db.Thuocs.Remove(thuoc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ExportToExcel()
        {
            var thuocs = db.Thuocs.ToList(); // Lấy tất cả các dữ liệu từ cơ sở dữ liệu

            if (thuocs == null || thuocs.Count == 0)
            {
                return HttpNotFound();
            }

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("QLThuoc");

            // Thiết lập dữ liệu trong file Excel
            ws.Cell(1, 1).Value = "ID Thuốc";
            ws.Cell(1, 2).Value = "Tên Thuốc";
            ws.Cell(1, 3).Value = "Mô tả";
            ws.Cell(1, 4).Value = "Giá";
            ws.Cell(1, 5).Value = "Ngày Sản Xuất";
            ws.Cell(1, 6).Value = "Hạn Sử Dụng";
            ws.Cell(1, 7).Value = "Số Lượng";
            ws.Cell(1, 8).Value = "Thành Phần";

            int row = 2;
            foreach (var thuoc in thuocs)
            {
                ws.Cell(row, 1).Value = thuoc.Id_thuoc;
                ws.Cell(row, 2).Value = thuoc.Tenthuoc;
                ws.Cell(row, 3).Value = thuoc.Mota;
                ws.Cell(row, 4).Value = thuoc.Gia;
                ws.Cell(row, 5).Value = thuoc.NgaySX;
                ws.Cell(row, 6).Value = thuoc.HanSD;
                ws.Cell(row, 7).Value = thuoc.Soluong;
                ws.Cell(row, 8).Value = thuoc.Thanhphan;
                row++;
            }

            // Lưu file Excel
            // Lưu file Excel vào một mảng byte
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Trả về file Excel đã tạo
            var fileName = "AllThuoc.xlsx";

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
