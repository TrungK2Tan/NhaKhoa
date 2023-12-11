using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NhaKhoa.Models;

namespace NhaKhoa.Areas.NhaSi.Controllers
{
    public class DonThuocController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: NhaSi/DonThuoc
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId(); 
            var donThuocs = db.DonThuocs
                .Include(d => d.PhieuDatLich)
                .Where(d => d.PhieuDatLich.IdNhaSi == currentUserId); // Adjust this line based on your actual foreign key property.

            return View(donThuocs.ToList());
        }

        // GET: NhaSi/DonThuoc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonThuoc donThuoc = db.DonThuocs.Find(id);
            if (donThuoc == null)
            {
                return HttpNotFound();
            }
            return View(donThuoc);
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
