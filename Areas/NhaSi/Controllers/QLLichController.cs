using Microsoft.AspNet.Identity;
using NhaKhoa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
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
                var thoiKhoaBieu = db.Thus.Where(u => u.ThoiKhoaBieux.Any(r => r.Id_Nhasi == currentUserId)).ToList();


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