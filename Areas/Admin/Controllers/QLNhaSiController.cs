using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System.Threading.Tasks;
using System.Text;
using PagedList;
using System.Globalization;

namespace NhaKhoa.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QLNhaSiController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();
        private readonly UserManager<ApplicationUser> _userManager;
        public QLNhaSiController()
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        // GET: Admin/QLNhanVien
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            // Lấy thông tin người dùng đã đăng nhập
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(userId);

            // Lấy danh sách người dùng thuộc vai trò "NhanVien"
            var nhanVienUsers = userManager.Users.Where(u => u.Roles.Any(r => r.RoleId == "2")).ToList();
            ViewBag.HinhAnh = user.HinhAnh;
            return View(nhanVienUsers);
        }


        // GET: Admin/QLNhaSi/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // GET: Admin/QLNhaSi/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/QLNhaSi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.UserName,
                    GioiTinh = model.GioiTinh,
                    DiaChi = model.DiaChi,
                    NgaySinh = model.NgaySinh,
                    NgheNghiep = model.NgheNghiep,
                    NgayTao = model.NgayTao,
                    CCCD = model.CCCD,
                    FullName = model.FullName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Gán quyền "NhaSi" cho tài khoản người dùng
                    await _userManager.AddToRoleAsync(user.Id, "NhaSi");
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            return View(model);
        }

        // GET: Admin/QLNhaSi/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Admin/QLNhaSi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,GioiTinh,DiaChi,Trangthai,Ngaysinh,NgheNghiep,NgayTao,Bangcap,CCCD,FullName")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: Admin/QLNhaSi/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Admin/QLNhaSi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
        //public ActionResult TKB()
        //{
        //    try
        //    {
        //        var thoiKhoaBieu = db.Thus.ToList();

        //        if (thoiKhoaBieu == null || !thoiKhoaBieu.Any())
        //        {
        //            // Xử lý khi không có dữ liệu
        //            return View("ErrorView"); // Thay "ErrorView" bằng tên view hiển thị thông báo lỗi
        //        }

        //        return View(thoiKhoaBieu);
        //    }
        //    catch (Exception)
        //    {
        //        // Xử lý exception, log và hiển thị thông báo lỗi
        //        return View("ErrorView"); // Thay "ErrorView" bằng tên view hiển thị thông báo lỗi
        //    }
        //}
        public ActionResult TKB(DateTime? selectedWeek)
        {
            try
            {
                // Lấy danh sách các ngày trong tuần và lịch làm việc từ cơ sở dữ liệu
                var danhSachThu = db.Thus.ToList();
                var danhSachThoiKhoaBieu = db.ThoiKhoaBieux.ToList();

                // Kiểm tra xem có dữ liệu để hiển thị không
                if (danhSachThu.Any() && danhSachThoiKhoaBieu.Any())
                {
                    DateTime startOfWeek = selectedWeek ?? DateTime.Now;

                    // Nếu có tuần đã chọn, lọc danh sách thời khóa biểu cho tuần đó
                    var filteredThoiKhoaBieu = danhSachThoiKhoaBieu
                        .Where(tkb => tkb.NgayLamViec.HasValue && tkb.NgayLamViec.Value.Date == startOfWeek.Date)
                        .OrderBy(e => e.Id_Thu)
                        .ThenBy(e => e.NgayLamViec)
                        .ToList();

                    // Lấy calendar hiện tại (GregorianCalendar)
                    GregorianCalendar calendar = new GregorianCalendar();

                    // Tạo mảng chứa các tuần
                    DateTime[] weeks = GetWeeksInYear(DateTime.Now.Year, calendar);

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
            DateTime[] weeks = new DateTime[calendar.GetWeekOfYear(new DateTime(year, 12, 31), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)];

            // Ngày đầu tiên của năm
            DateTime startDate = new DateTime(year, 1, 1);

            // Đếm số ngày đã được thêm vào mảng
            int daysAdded = 0;

            // Duyệt qua từng ngày trong năm
            for (int i = 0; i < 365; i++)
            {
                DateTime currentDate = startDate.AddDays(i);

                // Nếu là ngày đầu tiên của một tuần, thêm vào mảng
                if (currentDate.DayOfWeek == DayOfWeek.Monday)
                {
                    weeks[calendar.GetWeekOfYear(currentDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) - 1] = currentDate;
                    daysAdded++;
                }
            }

            return weeks;
        }
        public ActionResult ThemThoiKhoaBieu()
        {
            ViewBag.ListPhong = new SelectList(db.Phongs, "Id_Phong", "TenPhong");
            ViewBag.ListNhaSi = new SelectList(db.AspNetUsers.Where(u => u.AspNetRoles.Any(r => r.Id == "2")), "Id", "FullName");
            ViewBag.ListKhungGio = new SelectList(db.KhungGios, "Id_khunggio", "TenCa");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemThoiKhoaBieu(ThoiKhoaBieu thoiKhoaBieu)
        {
            if (ModelState.IsValid)
            {
                if (thoiKhoaBieu.NgayLamViec < DateTime.Now.Date)
                {
                    ModelState.AddModelError(string.Empty, "Lịch làm việc đã quá ngày, Vui lòng chọn lịch khác");
                    SetupDropdownLists();
                    return View(thoiKhoaBieu);
                }
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

    }
}