using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NhaKhoa.Areas.NhanVien.Controllers
{
    public class NhanVienController : Controller
    {
        // GET: NhanVien/NhanVien
        public ActionResult Index()
        {
            return View();
        }
    }
}