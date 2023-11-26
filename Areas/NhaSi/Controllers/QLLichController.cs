using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;
using System.Net;

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
            try
            {
                // Filter ThoiKhoaBieu based on the Id_Nhasi
                var thoiKhoaBieu = db.Thus.Where(u => u.ThoiKhoaBieux.Any(r => r.Id_Nhasi == currentUserId))
                    .Include(tkb => tkb.ThoiKhoaBieux.Select(t => t.PhieuDatLiches)) // Include PhieuDatLiches
            .ToList();


                if (thoiKhoaBieu == null || !thoiKhoaBieu.Any())
                {
                    // Xử lý khi không có dữ liệu
                    return View("ErrorView"); // Thay "ErrorView" bằng tên view hiển thị thông báo lỗi
                }

                return View(thoiKhoaBieu);
            }
            catch (Exception)
            {
                // Xử lý exception, log và hiển thị thông báo lỗi
                return View("ErrorView"); // Thay "ErrorView" bằng tên view hiển thị thông báo lỗi
            }
        }
        public ActionResult AddCalendar()
        {
            ViewBag.ListPhong = new SelectList(db.Phongs, "Id_Phong", "TenPhong");
            ViewBag.ListKhungGio = new SelectList(db.KhungGios, "Id_khunggio", "TenCa");

            return View();
        }
        // GET: NhaSi/ThoiKhoaBieux/Edit/5
        public ActionResult EditCalendar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThoiKhoaBieu thoiKhoaBieu = db.ThoiKhoaBieux.Find(id);
            if (thoiKhoaBieu == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Nhasi = new SelectList(db.AspNetUsers, "Id", "Email", thoiKhoaBieu.Id_Nhasi);
            ViewBag.Id_khunggio = new SelectList(db.KhungGios, "Id_khunggio", "TenCa", thoiKhoaBieu.Id_khunggio);
            ViewBag.Id_Phong = new SelectList(db.Phongs, "Id_Phong", "TenPhong", thoiKhoaBieu.Id_Phong);
            ViewBag.Id_Thu = new SelectList(db.Thus, "Id_Thu", "TenThu", thoiKhoaBieu.Id_Thu);
            return View(thoiKhoaBieu);
        }

        // POST: NhaSi/ThoiKhoaBieux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCalendar([Bind(Include = "Id_TKB,Id_Thu,Id_Phong,Id_khunggio,NgayLamViec,Id_Nhasi")] ThoiKhoaBieu thoiKhoaBieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thoiKhoaBieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_Nhasi = new SelectList(db.AspNetUsers, "Id", "Email", thoiKhoaBieu.Id_Nhasi);
            ViewBag.Id_khunggio = new SelectList(db.KhungGios, "Id_khunggio", "TenCa", thoiKhoaBieu.Id_khunggio);
            ViewBag.Id_Phong = new SelectList(db.Phongs, "Id_Phong", "TenPhong", thoiKhoaBieu.Id_Phong);
            ViewBag.Id_Thu = new SelectList(db.Thus, "Id_Thu", "TenThu", thoiKhoaBieu.Id_Thu);
            return View(thoiKhoaBieu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCalendar(ThoiKhoaBieu thoiKhoaBieu)
        {
            if (ModelState.IsValid)
            {
                // Get the currently logged-in user's ID
                string currentUserId = User.Identity.GetUserId();

                // Set the NhaSi's ID for the ThoiKhoaBieu entry
                thoiKhoaBieu.Id_Nhasi = currentUserId;
                // Lấy giá trị của trường NgayLamViec và IdThu từ đối tượng NgayVaThu
                NgayVaThu ngayVaThu = LayNgayVaThu(thoiKhoaBieu.NgayLamViec);

                // Gán giá trị của NgayLamViec và IdThu từ đối tượng NgayVaThu
                thoiKhoaBieu.NgayLamViec = ngayVaThu.NgayLamViec;
                thoiKhoaBieu.Id_Thu = ngayVaThu.IdThu; // Use IdThu instead of TenThuId

                // Kiểm tra trùng lặp trước khi thêm mới
                if (KiemTraTrungLich(thoiKhoaBieu))
                {
                    // Nếu trùng lịch, thêm lỗi vào ModelState và chuyển hướng quay lại view
                    ModelState.AddModelError(string.Empty, "Lịch làm việc đã trùng. Vui lòng chọn lịch khác.");
                    SetupDropdownLists(); // Gọi hàm này để cập nhật lại danh sách dropdown
                    return View(thoiKhoaBieu);
                }

                // Thêm mới vào cơ sở dữ liệu và chuyển hướng
                db.ThoiKhoaBieux.Add(thoiKhoaBieu);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu ModelState không hợp lệ, hiển thị lại view với thông báo lỗi và danh sách dropdown đã chọn
            SetupDropdownLists(); // Gọi hàm này để cập nhật lại danh sách dropdown
            return View(thoiKhoaBieu);
        }

        // Hàm kiểm tra trùng lịch
        private bool KiemTraTrungLich(ThoiKhoaBieu thoiKhoaBieu)
        {
            // Kiểm tra xem lịch làm việc mới có trùng với lịch làm việc đã tồn tại không
            return db.ThoiKhoaBieux.Any(t => t.Id_Phong == thoiKhoaBieu.Id_Phong &&
                                            t.Id_Nhasi == thoiKhoaBieu.Id_Nhasi &&
                                            t.Id_khunggio == thoiKhoaBieu.Id_khunggio &&
                                            t.NgayLamViec == thoiKhoaBieu.NgayLamViec);
        }

        // Hàm thiết lập danh sách dropdown
        private void SetupDropdownLists()
        {
            ViewBag.ListPhong = new SelectList(db.Phongs.Select(p => new { p.Id_Phong, p.TenPhong }), "Id_Phong", "TenPhong");
            ViewBag.ListNhaSi = new SelectList(db.AspNetUsers.Where(u => u.AspNetRoles.Any(r => r.Id == "2")), "Id", "FullName");
            ViewBag.ListKhungGio = new SelectList(db.KhungGios, "Id_khunggio", "TenCa");
        }


        public class NgayVaThu
        {
            public DateTime? NgayLamViec { get; set; }
            public int IdThu { get; set; }
        }

        private NgayVaThu LayNgayVaThu(DateTime? ngayLamViec)
        {
            if (ngayLamViec.HasValue)
            {
                // Lấy thứ từ ngày
                string[] daysOfWeek = { "Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy" };
                string tenThu = daysOfWeek[(int)ngayLamViec.Value.DayOfWeek];

                // Lấy Id của TenThu từ bảng Thu
                int idThu = db.Thus.Single(t => t.TenThu == tenThu).Id_Thu;

                // Trả về đối tượng chứa ngày và Id của TenThu
                return new NgayVaThu { NgayLamViec = ngayLamViec, IdThu = idThu };
            }
            else
            {
                // Trường hợp ngày làm việc là null
                return new NgayVaThu { NgayLamViec = null, IdThu = -1 }; // Modify this default value based on your actual model
            }
        }
        public ActionResult ViewCalendar(int? page)
        {
            // Get the currently logged-in user's ID
            string currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            ViewBag.TenNhaSi = user.FullName;
            ViewBag.HinhAnh = user.HinhAnh;
            try
            {
                var thoiKhoaBieu = db.Thus.ToList();

                if (!thoiKhoaBieu.Any())
                {
                    ViewBag.ErrorMessage = "Không có dữ liệu để hiển thị.";
                    return View(thoiKhoaBieu.ToPagedList(1, 7)); // Mặc định hiển thị trang 1, mỗi trang 10 phần tử
                }

                int pageSize = 7; // Số lượng mục hiển thị trên mỗi trang
                int pageNumber = (page ?? 1);

                return View(thoiKhoaBieu.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại sau.";

                return View("ErrorView");
            }
        }
    }
}