using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using PagedList;

namespace NhaKhoa.Areas.Admin.Controllers
{
    public class QLTinTucController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();

        // GET: Admin/QLTinTuc
        public ActionResult Index(int? page)
        {
            const int pageSize = 1; // Adjust the number of items per page as needed

            var tinTucs = db.TinTucs.Include(t => t.AspNetUser).Include(t => t.DanhGias);

            int pageNumber = (page ?? 1); // If page is null, default to page 1
            var paginatedTinTucs = tinTucs.OrderBy(t => t.Id_tintuc).ToPagedList(pageNumber, pageSize);

            return View(paginatedTinTucs);
        }

        // GET: Admin/QLTinTuc/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            return View(tinTuc);
        }

        // GET: Admin/QLTinTuc/Create
        public ActionResult Create()
        {
            var currentUserId = User.Identity.GetUserId();
            // Set ViewBag.Id_admin to the current user's login ID
            ViewBag.Id_admin = currentUserId;
            return View();
        }

        // POST: Admin/QLTinTuc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_tintuc,Tieude,Noidung,Hinhanh,Ngaygiotao,Id_admin,Id_Benhnhan,Thich,Id_danhgia")] TinTuc tinTuc, HttpPostedFileBase HinhanhFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (HinhanhFile != null && HinhanhFile.ContentLength > 0)
                {
                    // Ensure the target folder exists
                    var folderPath = Server.MapPath("~/images/tintuc/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Generate a unique file name to avoid conflicts
                    var fileName = Path.GetFileNameWithoutExtension(HinhanhFile.FileName)
                                   + "_" + Guid.NewGuid().ToString().Substring(0, 8)
                                   + Path.GetExtension(HinhanhFile.FileName);

                    var path = Path.Combine(folderPath, fileName);
                    HinhanhFile.SaveAs(path);

                    // Set the file path to the model property
                    tinTuc.Hinhanh = fileName;
                }

                tinTuc.Id_admin = User.Identity.GetUserId();
                db.TinTucs.Add(tinTuc);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            // Set ViewBag.Id_admin to the current user's login ID
            ViewBag.Id_admin = User.Identity.GetUserId();

            return View(tinTuc);
        }

        // GET: Admin/QLTinTuc/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_admin = new SelectList(db.AspNetUsers, "Id", "Email", tinTuc.Id_admin);
            return View(tinTuc);
        }

        // POST: Admin/QLTinTuc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_tintuc,Tieude,Noidung,Hinhanh,Ngaygiotao,Id_admin,Id_Benhnhan,Thich,Id_danhgia")] TinTuc tinTuc, HttpPostedFileBase HinhanhFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (HinhanhFile != null && HinhanhFile.ContentLength > 0)
                {
                    // Ensure the target folder exists
                    var folderPath = Server.MapPath("~/images/tintuc/");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // Generate a unique file name to avoid conflicts
                    var fileName = Path.GetFileNameWithoutExtension(HinhanhFile.FileName)
                                   + "_" + Guid.NewGuid().ToString().Substring(0, 8)
                                   + Path.GetExtension(HinhanhFile.FileName);

                    var path = Path.Combine(folderPath, fileName);
                    HinhanhFile.SaveAs(path);

                    // Set the file path to the model property
                    tinTuc.Hinhanh = fileName;
                }
                tinTuc.Id_admin = User.Identity.GetUserId();
                db.Entry(tinTuc).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_admin = new SelectList(db.AspNetUsers, "Id", "Email", tinTuc.Id_admin);
            return View(tinTuc);
        }

        // GET: Admin/QLTinTuc/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TinTuc tinTuc = db.TinTucs.Find(id);
            if (tinTuc == null)
            {
                return HttpNotFound();
            }
            return View(tinTuc);
        }

        // POST: Admin/QLTinTuc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TinTuc tinTuc = db.TinTucs.Find(id);
            db.TinTucs.Remove(tinTuc);
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
    }
}
