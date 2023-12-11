using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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

        public ActionResult LapDonThuoc(int? id_phieudat)
        {
            if (id_phieudat == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PhieuDatLich phieuDatLich = db.PhieuDatLiches.Find(id_phieudat);

            if (phieuDatLich == null)
            {
                return HttpNotFound();
            }

            List<ThuocCheckBox> thuocs = db.Thuocs
                .Select(t => new ThuocCheckBox
                {
                    Id_thuoc = t.Id_thuoc,
                    Tenthuoc = t.Tenthuoc,
                    Selected = false
                })
                .ToList();

            LapDonThuocViewModel viewModel = new LapDonThuocViewModel
            {
                DonThuoc = new DonThuoc
                {
                    Id_phieudat = id_phieudat,
                    NgayGio = DateTime.Now,
                    // Initialize the total quantity to 0
                    Soluong = 0
                },
                Thuocs = thuocs
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LapDonThuoc(LapDonThuocViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // If ModelState is not valid, return to the view with the entered data
                viewModel.Thuocs = db.Thuocs
                    .Select(t => new ThuocCheckBox
                    {
                        Id_thuoc = t.Id_thuoc,
                        Tenthuoc = t.Tenthuoc,
                        Selected = false
                    })
                    .ToList();

                return View(viewModel);
            }

            try
            {
                // Set the appointment for the prescription
                viewModel.DonThuoc.PhieuDatLich = db.PhieuDatLiches.Find(viewModel.DonThuoc.Id_phieudat);

                // Save the prescription to the database
                db.DonThuocs.Add(viewModel.DonThuoc);

                // Initialize total quantity and total cost
                viewModel.DonThuoc.Soluong = 0;
                viewModel.DonThuoc.TongTien = 0;

                // Update the quantity and cost of selected medicines in the prescription
                foreach (var thuoc in viewModel.Thuocs.Where(t => t.Selected))
                {
                    var giaThuoc = db.Thuocs.Find(thuoc.Id_thuoc)?.Gia ?? 0;
                    var chiTietThuoc = new ChiTietThuoc
                    {
                        Id_thuoc = thuoc.Id_thuoc,
                        soluong = thuoc.SoLuong,
                        Gia = giaThuoc * thuoc.SoLuong, // Tính giá dựa trên số lượng và giá từ bảng Thuoc
                    };

                    viewModel.DonThuoc.ChiTietThuocs.Add(chiTietThuoc);

                    // Update the total quantity and total cost in DonThuoc
                    viewModel.DonThuoc.Soluong += thuoc.SoLuong;
                    viewModel.DonThuoc.TongTien += chiTietThuoc.Gia;
                }
                // Update the TrangThai of PhieuDatLich to true
                viewModel.DonThuoc.PhieuDatLich.TrangThai= true;

                db.SaveChanges();

                // Redirect to a relevant page, e.g., the details page for the created prescription
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions (e.g., unique constraint violation) more specifically
                ModelState.AddModelError(string.Empty, "An error occurred while saving the prescription.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions appropriately, log the error, etc.
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while saving the prescription.");
            }

            // If an error occurred, return to the view with the entered data
            viewModel.Thuocs = db.Thuocs
                .Select(t => new ThuocCheckBox
                {
                    Id_thuoc = t.Id_thuoc,
                    Tenthuoc = t.Tenthuoc,
                    Selected = false
                })
                .ToList();

            return View(viewModel);
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
