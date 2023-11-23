using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace NhaKhoa.Controllers
{
    public class HomeController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();
        public ActionResult Index()
        {
            var nhaSiList = db.AspNetUsers
                   .Where(u => u.AspNetRoles.Any(r => r.Name == "NhaSi"))
                   .ToList();

            return View(nhaSiList);
        }
        public ActionResult DetailsDoctor(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AspNetUser dental = db.AspNetUsers
                .Where(u => u.AspNetRoles.Any(r => r.Name == "NhaSi"))
                .FirstOrDefault(u => u.Id == id);

            if (dental == null)
            {
                return HttpNotFound();
            }

            return View(dental);
        }
        public ActionResult About()
        {
           
            return View();
        }

        public ActionResult Contact()
        {
            

            return View();
        }
        
        public ActionResult Service()
        {

            return View();
        }
        public ActionResult BlogGrid(int? page)
        {
            const int pageSize = 3; // Adjust the number of items per page as needed

            // Lấy danh sách các bài viết từ database và sắp xếp theo Id_tintuc
            var tintucs = db.TinTucs.OrderBy(b => b.Id_tintuc);

            int pageNumber = (page ?? 1); // If page is null, default to page 1
            var paginatedTinTucs = tintucs.OrderBy(t => t.Id_tintuc).ToPagedList(pageNumber, pageSize);

            return View(paginatedTinTucs);
        }
        public ActionResult BlogDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TinTuc tintuc = db.TinTucs.Find(id);

            if (tintuc == null)
            {
                return HttpNotFound();
            }

            return View(tintuc);
        }

        [Authorize]
        public ActionResult Appointment( )
        {
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
        public ActionResult Appointment([Bind(Include = "Id_Phieudat,NgayKham,Gia,Id_hinhthuc,IdNhaSi,IdBenhNhan,Id_TKB,STT")] PhieuDatLich DatLich)
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
                var numberOfAppointments = db.PhieuDatLiches.Count(l => l.IdNhaSi == DatLich.IdNhaSi && l.NgayKham.HasValue && DbFunctions.TruncateTime(l.NgayKham) == DbFunctions.TruncateTime(DatLich.NgayKham));

                if (numberOfAppointments >= 2)
                {
                    ModelState.AddModelError("", "Nha sĩ này đã đủ số lượng lịch hẹn cho khung giờ này. Vui lòng chọn nha sĩ khác.");
                    
                    return View(DatLich);
                }

                // Your existing code to save the appointment
                string currentUserId = User.Identity.GetUserId();
                DatLich.IdBenhNhan = currentUserId;
                // Add the appointment to the database
                db.PhieuDatLiches.Add(DatLich);
                db.SaveChanges();
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
        [HttpGet]
        public ActionResult GetNhaSiList(DateTime selectedDate)
        {
            var nhaSiList = db.ThoiKhoaBieux
                .Where(t => t.NgayLamViec == selectedDate)
                .Select(t => new { IdNhaSi = t.Id_Nhasi, TenNhaSi = t.AspNetUser.FullName })
                .ToList();

            return Json(nhaSiList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Search(string keyword)
        {
            // Tìm kiếm theo từ khóa keyword trong tên nha sĩ
            var nhaSiResults = db.AspNetUsers.Where(u => u.AspNetRoles.Any(r => r.Name == "NhaSi")).Where(n => n.FullName.Contains(keyword)).ToList();

            // Tìm kiếm theo từ khóa keyword trong tên dịch vụ
            var tintucResults = db.TinTucs.Where(d => d.Tieude.Contains(keyword)).ToList();

            // Tạo một ViewModel để chứa kết quả tìm kiếm
            var searchResults = new SearchViewModel
            {
                Keyword = keyword,
                NhaSiResults = nhaSiResults,
                TinTucResults = tintucResults
            };

            // Kiểm tra kết quả tìm kiếm để xác định view cần hiển thị
            if (nhaSiResults.Any() || tintucResults.Any())
            {
                // Gửi kết quả tìm kiếm cho view "SearchResults"
                return View("SearchResults", searchResults);
            }
            else
            {
                // Không có kết quả tìm kiếm, gửi thông báo cho view "NoResults"
                return View("NoResults", searchResults);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(DanhGia review)
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Get the current user's ID
                string userId = User.Identity.GetUserId(); // Assuming you have User.Identity available

                // Set the user ID in the review object
                review.Id_Benhnhan = userId;
                // Lưu đánh giá và bình luận vào cơ sở dữ liệu
                db.DanhGias.Add(review);
                db.SaveChanges();

                return RedirectToAction("BlogDetail", new { id = review.Id_tintuc });
            }
            else{ 
            // Nếu dữ liệu không hợp lệ, quay lại trang Details với thông tin nha sĩ và các đánh giá và bình luận đã nhập trước đó
            TinTuc tintuc = db.TinTucs.Find(review.Id_tintuc);
            tintuc.DanhGias = db.DanhGias.Where(r => r.Id_tintuc == review.Id_tintuc).ToList();
            return View("BlogDetail", tintuc);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReviewNhaSi(DanhGiaNhaSi reviewnhasi)
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Get the current user's ID
                string userId = User.Identity.GetUserId(); // Assuming you have User.Identity available

                // Set the user ID in the review object
                reviewnhasi.Id_Benhnhan = userId;
                // Lưu đánh giá và bình luận vào cơ sở dữ liệu
                db.DanhGiaNhaSis.Add(reviewnhasi);
                db.SaveChanges();

                return RedirectToAction("DetailsDoctor", new { id = reviewnhasi.Id_Nhasi });
            }
            else
            {
                // Nếu dữ liệu không hợp lệ, quay lại trang Details với thông tin nha sĩ và các đánh giá và bình luận đã nhập trước đó
                AspNetUser nhasi = db.AspNetUsers.Find(reviewnhasi.Id_Nhasi);
                nhasi.DanhGiaNhaSis = db.DanhGiaNhaSis.Where(r => r.Id_Nhasi == reviewnhasi.Id_Nhasi).ToList();
                return View("DetailsDoctor", nhasi);
            }
        }
        [Authorize]
        public ActionResult BookingView()
        {

            // Lấy ID đăng nhập của người dùng hiện tại
            string currentUserId = User.Identity.GetUserId();
            // Lấy danh sách lịch hẹn dựa trên ID đăng nhập
            var lichHens = db.PhieuDatLiches
                .Where(l => l.IdBenhNhan == currentUserId).OrderBy(l => l.NgayKham).ToList();
            return View(lichHens);
        }
        [HttpPost]
        public ActionResult CancelBooking(int bookingId)
        {
            // Kiểm tra trạng thái của đặt lịch và thực hiện hủy nếu hợp lệ
            var booking = db.PhieuDatLiches.Find(bookingId);
            if (booking != null && booking.TrangThai == false) // Thay đổi ở đây
            {
                db.PhieuDatLiches.Remove(booking);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
    }
}