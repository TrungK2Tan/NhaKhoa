using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BotDetect.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NhaKhoa.Models;

namespace NhaKhoa.Areas.NhanVien.Controllers
{
    public class QLBenhNhanController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public QLBenhNhanController()
        {
        }

        public QLBenhNhanController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: NhanVien/QLYNVien
        public ActionResult Index()
        {
            // Lấy thông tin người dùng đã đăng nhập
            var userId = User.Identity.GetUserId();
            var user = db.AspNetUsers.Find(userId);
            ViewBag.TenNhanVien = user.FullName;
            // Truy vấn cơ sở dữ liệu để lấy thông tin bệnh nhân kết hợp thông tin người dùng và vai trò
            var benhNhanList = db.AspNetUsers
                .Where(u => u.AspNetRoles.Any(r => r.Id == "4"))
                .ToList();

            // Truyền danh sách thông tin bệnh nhân cho view
            return View(benhNhanList);
        }

        // GET: NhanVien/QLBenhNhan/Details/5
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
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //su dung captcha để đăng ký 
        [CaptchaValidation("CaptchaCode", "registerCaptcha", "Mã xác nhận không đùng! ")]
        public async Task<ActionResult> Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    DiaChi = model.DiaChi,
                    NgheNghiep = model.NgheNghiep,
                    CCCD = model.CCCD,
                    GioiTinh = model.GioiTinh,
                    NgaySinh = model.NgaySinh,
                    NgayTao = model.NgayTao,
                    FullName = model.FullName
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "BenhNhan");

                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        // GET: NhanVien/QLBenhNhan/Edit/5
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

        // POST: NhanVien/QLBenhNhan/Edit/5
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
        [Authorize]
        public ActionResult Booking(string id)
        {
            PhieuDatLich phieuDatLich = db.PhieuDatLiches.FirstOrDefault(u => u.IdBenhNhan == id);
            ViewBag.IdBenhNhan = id;
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
        public ActionResult Booking([Bind(Include = "Id_Phieudat,NgayKham,Gia,Id_hinhthuc,IdNhaSi,IdBenhNhan,Id_TKB,STT,TrangThai,TrangThaiThanhToan")] PhieuDatLich DatLich,string id)
        {
            if (ModelState.IsValid)
            {
                // Gán giá trị cố định 150 cho trường Gia
                DatLich.Gia = 150000;
                // Lấy Id_TKB tương ứng với NgayKham từ cơ sở dữ liệu
                DatLich.Id_TKB = db.ThoiKhoaBieux
                    .Where(t => t.NgayLamViec == DatLich.NgayKham)
                    .Select(t => t.Id_TKB)
                    .FirstOrDefault();
                DatLich.TrangThai = false;
                DatLich.TrangThaiThanhToan = false;
                // Calculate STT
                DatLich.STT = CalculateSTT(DatLich.NgayKham, DatLich.IdNhaSi);

                DatLich.IdBenhNhan =  id;
                var numberOfAppointments = db.PhieuDatLiches.Count(l => l.IdNhaSi == DatLich.IdNhaSi && l.NgayKham.HasValue && DbFunctions.TruncateTime(l.NgayKham) == DbFunctions.TruncateTime(DatLich.NgayKham));

                if (numberOfAppointments >= 2)
                {
                    ModelState.AddModelError("", "Nha sĩ này đã đủ số lượng lịch hẹn cho ngày này. Vui lòng chọn nha sĩ khác.");

                    return View(DatLich);
                }
                var appointmentvalue = db.PhieuDatLiches.Count(l => l.IdBenhNhan == id && l.Id_TKB.HasValue && l.NgayKham.HasValue && DbFunctions.TruncateTime(l.NgayKham) == DbFunctions.TruncateTime(DatLich.NgayKham));
                if (appointmentvalue >= 1)
                {
                    ModelState.AddModelError("", "Bạn đã đặt lịch này. Vui lòng hủy lịch cũ nếu bạn muốn đặt lịch mới");

                    return View(DatLich);
                }
                // Add the appointment to the database
                db.PhieuDatLiches.Add(DatLich);
                db.SaveChanges();
                if (DatLich.Id_hinhthuc == 3)
                {
                    // Set TrangThaiThanhToan to true
                    DatLich.TrangThaiThanhToan = true;
                    // Update the appointment in the database
                    db.Entry(DatLich).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "QLBenhNhan");
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
