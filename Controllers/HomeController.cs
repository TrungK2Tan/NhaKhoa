using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaKhoa.Controllers
{
    public class HomeController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult Service()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Appointment([Bind(Include = "Id_Phieudat,NgayKham,Gia,Id_hinhthuc,IdNhaSi,IdBenhNhan,Id_TKB")] PhieuDatLich DatLich)
        {   
            if (ModelState.IsValid)
            {
                // Gán giá trị cố định 150 cho trường Gia
                DatLich.Gia = 150;
                string currentUserId = User.Identity.GetUserId();
                DatLich.IdBenhNhan = currentUserId;
                db.PhieuDatLiches.Add(DatLich);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            // Nếu ModelState không hợp lệ, tái sử dụng SelectList cho dropdown
            var hinhThucList = db.HinhThucThanhToans.ToList();
            SelectList hinhThucSelectList = new SelectList(hinhThucList, "Id_hinhthuc", "TenHinhThuc");
            ViewBag.HinhThucList = hinhThucSelectList;
            // Đặt SelectList vào ViewBag hoặc mô hình
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
}