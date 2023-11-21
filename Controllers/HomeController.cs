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

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Appointment([Bind(Include = "Id_Phieudat,NgayKham,Gia,Id_hinhthuc,IdNhaSi,IdBenhNhan,Id_Thu")] PhieuDatLich DatLich)
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