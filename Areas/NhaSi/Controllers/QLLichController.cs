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
using System.Globalization;

namespace NhaKhoa.Areas.NhaSi.Controllers
{
    public class QLLichController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();
        // GET: NhaSi/QLLich
        public ActionResult Index(DateTime? selectedWeek)
        {

            // Get the currently logged-in user's ID
            string currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            ViewBag.TenNhaSi = user.FullName;
            ViewBag.HinhAnh = user.HinhAnh;
            ViewBag.CurrentUserId = currentUserId;
            try
            {
                // Lấy danh sách các ngày trong tuần và lịch làm việc từ cơ sở dữ liệu
                var danhSachThu = db.Thus.ToList();
                var danhSachThoiKhoaBieu = db.ThoiKhoaBieux.Where(tkb => tkb.Id_Nhasi == currentUserId).OrderBy(e => e.Id_Thu).ThenBy(e => e.NgayLamViec).ToList();

                // Kiểm tra xem có dữ liệu để hiển thị không
                if (danhSachThu.Any() || danhSachThoiKhoaBieu.Any())
                {
                    //DateTime startOfWeek = selectedWeek ?? DateTime.Now;
                    var now = DateTime.Now;
                    var daysUntilMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                    var monday = now.AddDays(-daysUntilMonday);
                    DateTime startOfWeek = selectedWeek ?? monday;
                    // Nếu có tuần đã chọn, lọc danh sách thời khóa biểu cho tuần đó
                    var filteredThoiKhoaBieu = danhSachThoiKhoaBieu
                        .Where(tkb => tkb.NgayLamViec.HasValue && tkb.NgayLamViec.Value.Date == startOfWeek.Date)
                        .OrderBy(e => e.Id_Thu)
                        .ThenBy(e => e.NgayLamViec)
                        .ToList();

                    // Lấy calendar hiện tại (GregorianCalendar)
                    GregorianCalendar calendar = new GregorianCalendar();

                    // Tạo mảng chứa các tuần
                    DateTime[] weeks = GetWeeksInYear(startOfWeek.Year, calendar);

                    // Tạo ViewModel
                    var viewModel = new ThoiKhoaBieuViewModel
                    {
                        DanhSachThu = danhSachThu,
                        DanhSachThoiKhoaBieu = filteredThoiKhoaBieu,
                        weeks = weeks,
                        SelectedWeek = startOfWeek
                    };

                    // Trả về view với ViewModel
                    return View(viewModel);
                }
                else
                {
                    // Nếu không có dữ liệu, hiển thị thông báo lỗi
                    ViewBag.ErrorMessage = "Không có dữ liệu để hiển thị.";
                    return View("ErrorView");
                }
            }
            catch (Exception ex)
            {
                // Log exception
                ViewBag.ErrorMessage = $"Đã xảy ra lỗi khi lấy dữ liệu: {ex.Message}";
                return View("ErrorView");
            }
           
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
        public ActionResult ViewCalendar(DateTime? selectedWeek)
        {
            // Get the currently logged-in user's ID
            string currentUserId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(currentUserId);
            ViewBag.TenNhaSi = user.FullName;
            ViewBag.HinhAnh = user.HinhAnh;
            ViewBag.CurrentUserId = currentUserId;
            try
            {
                // Lấy danh sách các ngày trong tuần và lịch làm việc từ cơ sở dữ liệu
                var danhSachThu = db.Thus.ToList();
                var danhSachThoiKhoaBieu = db.ThoiKhoaBieux.OrderBy(e => e.Id_Thu).ThenBy(e => e.NgayLamViec).ToList();

                // Kiểm tra xem có dữ liệu để hiển thị không
                if (danhSachThu.Any() || danhSachThoiKhoaBieu.Any())
                {
                    //DateTime startOfWeek = selectedWeek ?? DateTime.Now;
                    var now = DateTime.Now;
                    var daysUntilMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                    var monday = now.AddDays(-daysUntilMonday);
                    DateTime startOfWeek = selectedWeek ?? monday;
                    // Nếu có tuần đã chọn, lọc danh sách thời khóa biểu cho tuần đó
                    var filteredThoiKhoaBieu = danhSachThoiKhoaBieu
                        .Where(tkb => tkb.NgayLamViec.HasValue && tkb.NgayLamViec.Value.Date == startOfWeek.Date)
                        .OrderBy(e => e.Id_Thu)
                        .ThenBy(e => e.NgayLamViec)
                        .ToList();

                    // Lấy calendar hiện tại (GregorianCalendar)
                    GregorianCalendar calendar = new GregorianCalendar();

                    // Tạo mảng chứa các tuần
                    DateTime[] weeks = GetWeeksInYear(startOfWeek.Year, calendar);
                    // Check if the selected week is beyond the next year
                    if (startOfWeek.Year > DateTime.Now.Year + 1)
                    {
                        // Redirect to the main page
                        return RedirectToAction("ViewCalendar", "QLLich");
                    }
                    // Tạo ViewModel
                    var viewModel = new ThoiKhoaBieuViewModel
                    {
                        DanhSachThu = danhSachThu,
                        DanhSachThoiKhoaBieu = filteredThoiKhoaBieu,
                        weeks = weeks,
                        SelectedWeek = startOfWeek
                    };

                    // Trả về view với ViewModel
                    return View(viewModel);
                }
                else
                {
                    // Nếu không có dữ liệu, hiển thị thông báo lỗi
                    ViewBag.ErrorMessage = "Không có dữ liệu để hiển thị.";
                    return View("ErrorView");
                }
            }
            catch (Exception ex)
            {
                // Log exception
                ViewBag.ErrorMessage = $"Đã xảy ra lỗi khi lấy dữ liệu: {ex.Message}";
                return View("ErrorView");
            }
        }


        static DateTime[] GetWeeksInYear(int year, GregorianCalendar calendar)
        {
            int totalWeeks = calendar.GetWeekOfYear(new DateTime(year, 12, 31), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            // Limit the weeks to the next year
            int nextYear = year + 1;
            DateTime[] weeks = new DateTime[totalWeeks];

            // Ngày đầu tiên của năm
            DateTime startDate = new DateTime(year, 1, 1);

            // Đếm số ngày đã được thêm vào mảng
            int daysAdded = 0;

            // Duyệt qua từng ngày trong năm
            for (int i = 0; i < totalWeeks * 7; i++)
            {
                DateTime currentDate = startDate.AddDays(i);

                // Nếu là ngày đầu tiên của một tuần và nằm trong năm và năm sau, thêm vào mảng
                if (currentDate.DayOfWeek == DayOfWeek.Monday && currentDate.Year <= nextYear)
                {
                    weeks[calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) - 1] = currentDate;
                    daysAdded++;
                }

                // Nếu lịch vượt quá năm tiếp theo, kết thúc vòng lặp
                if (currentDate.Year > nextYear)
                {
                    break;
                }
            }

            return weeks;
        }



    }
}