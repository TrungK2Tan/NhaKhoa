using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaKhoa.Controllers
{
    public class BookingDentistController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();
        // GET: BookingDentist
        [Authorize]
        public ActionResult Appointment(DateTime NgayKham, string TenNhaSi,string idnhasi)
        {
            ViewBag.NgayKham = NgayKham;
            ViewBag.TenNhaSi = TenNhaSi;
            ViewBag.Idnhasi = idnhasi;
            // Truy vấn cơ sở dữ liệu để lấy danh sách Id_hinhthuc
            var hinhThucList = db.HinhThucThanhToans.ToList();
            // Tạo SelectList từ danh sách
            SelectList hinhThucSelectList = new SelectList(hinhThucList, "Id_hinhthuc", "TenHinhThuc");
            // Đặt SelectList vào ViewBag hoặc mô hình
            ViewBag.HinhThucList = hinhThucSelectList;
            var nhaSiList = db.AspNetUsers
                     .Where(u => u.AspNetRoles.Any(r => r.Name == "NhaSi"))
                     .Select(u => new { IdNhaSi = u.Id, TenNhaSi = u.FullName })
                     .ToList();
            SelectList nhaSiSelectList = new SelectList(nhaSiList, "IdNhaSi", "TenNhaSi");
            ViewBag.NhaSiList = nhaSiSelectList;
            // Populate the dates from ThoiKhoaBieu
            var availableDates = db.ThoiKhoaBieux.Select(t => t.NgayLamViec).Distinct().ToList();
            ViewBag.AvailableDates = new SelectList(availableDates);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Appointment([Bind(Include = "Id_Phieudat,NgayKham,Gia,Id_hinhthuc,IdNhaSi,IdBenhNhan,Id_TKB,STT,TrangThai,TrangThaiThanhToan")] PhieuDatLich DatLich)
        {
            if (ModelState.IsValid)
            {
                // Gán giá trị cố định 150 cho trường Gia
                DatLich.Gia = 150;
                // Lấy Id_TKB tương ứng với NgayKham từ cơ sở dữ liệu
                DatLich.Id_TKB = db.ThoiKhoaBieux
                    .Where(t => t.NgayLamViec == DatLich.NgayKham)
                    .Select(t => t.Id_TKB)
                    .FirstOrDefault();
                DatLich.TrangThai = false;
                DatLich.TrangThaiThanhToan = false;
                // Calculate STT
                DatLich.STT = CalculateSTT(DatLich.NgayKham, DatLich.IdNhaSi);
                //var numberOfAppointments = db.PhieuDatLiches.Count(l => l.IdNhaSi == DatLich.IdNhaSi && l.NgayKham.HasValue && DbFunctions.TruncateTime(l.NgayKham) == DbFunctions.TruncateTime(DatLich.NgayKham));

                //if (numberOfAppointments >= 2)
                //{
                //    ModelState.AddModelError("", "Nha sĩ này đã đủ số lượng lịch hẹn cho khung giờ này. Vui lòng chọn nha sĩ khác.");

                //    return View(DatLich);
                //}

                // Your existing code to save the appointment
                string currentUserId = User.Identity.GetUserId();
                DatLich.IdBenhNhan = currentUserId;
                // Add the appointment to the database
                db.PhieuDatLiches.Add(DatLich);
                db.SaveChanges();
                if (DatLich.Id_hinhthuc == 1)
                {
                    //DatLich.TrangThaiThanhToan = true; // Gán TrangThai là false
                    return RedirectToAction("Payment", "Home", new { order = DatLich.Id_Phieudat });
                }
                if (DatLich.Id_hinhthuc == 2)
                {
                    return RedirectToAction("PaymentVNPay", "Home", new { order = DatLich.Id_Phieudat });
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Re-populate dropdown lists in case of validation errors
                var hinhThucList = db.HinhThucThanhToans.ToList();
                SelectList hinhThucSelectList = new SelectList(hinhThucList, "Id_hinhthuc", "TenHinhThuc");
                ViewBag.HinhThucList = hinhThucSelectList;

                var nhaSiList = db.AspNetUsers
                    .Where(u => u.AspNetRoles.Any(r => r.Name == "NhaSi"))
                    .Select(u => new { IdNhaSi = u.Id, TenNhaSi = u.FullName })
                    .ToList();
                SelectList nhaSiSelectList = new SelectList(nhaSiList, "IdNhaSi", "TenNhaSi");
                ViewBag.NhaSiList = nhaSiSelectList;
                return View(DatLich);
            }
        }
        private int CalculateSTT(DateTime? ngayKham, string idNhaSi)
        {
            // Get the number of appointments for the selected date and doctor
            int numberOfAppointments = db.PhieuDatLiches
                .Count(l => l.IdNhaSi == idNhaSi && l.NgayKham == ngayKham);

            // Increment the number to get the next sequence number
            return numberOfAppointments + 1;
        }
        [HttpGet]
        public ActionResult GetNhaSiList(DateTime selectedDate)
        {
            var nhaSiList = db.ThoiKhoaBieux
                .Where(t => t.NgayLamViec == selectedDate)
                .Select(t => new { IdNhaSi = t.Id_Nhasi, TenNhaSi = t.AspNetUser.FullName })
                .ToList();

            return Json(nhaSiList, JsonRequestBehavior.AllowGet);
        }
    }

}