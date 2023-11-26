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
using MoMo;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Migrations;

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
        public ActionResult Appointment()
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
            // Populate the dates from ThoiKhoaBieu
            var availableDates = db.ThoiKhoaBieux.Select(t => t.NgayLamViec).Distinct().ToList();
            ViewBag.AvailableDates = new SelectList(availableDates);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Appointment([Bind(Include = "Id_Phieudat,NgayKham,Gia,Id_hinhthuc,IdNhaSi,IdBenhNhan,Id_TKB,STT,TrangThai,TrangThaiThanhToan")] PhieuDatLich DatLich)
        {
            if (ModelState.IsValid)
            {
                // Gán giá trị cố định 150 cho trường Gia
                DatLich.Gia = 150;
                // Lấy Id_TKB tương ứng với NgayKham từ cơ sở dữ liệu
                DatLich.Id_TKB = db.ThoiKhoaBieux
                    .Where(t => t.NgayLamViec == DatLich.NgayKham)
                    .Select(t => t.Id_TKB)
                    .FirstOrDefault();
                DatLich.TrangThai = false;
                DatLich.TrangThaiThanhToan = false;
                // Calculate STT
                DatLich.STT = CalculateSTT(DatLich.NgayKham, DatLich.IdNhaSi);
                //var numberOfAppointments = db.PhieuDatLiches.Count(l => l.IdNhaSi == DatLich.IdNhaSi && l.NgayKham.HasValue && DbFunctions.TruncateTime(l.NgayKham) == DbFunctions.TruncateTime(DatLich.NgayKham));

                //if (numberOfAppointments >= 2)
                //{
                //    ModelState.AddModelError("", "Nha sĩ này đã đủ số lượng lịch hẹn cho khung giờ này. Vui lòng chọn nha sĩ khác.");

                //    return View(DatLich);
                //}

                // Your existing code to save the appointment
                string currentUserId = User.Identity.GetUserId();
                DatLich.IdBenhNhan = currentUserId;
                // Add the appointment to the database
                db.PhieuDatLiches.Add(DatLich);
                db.SaveChanges();
                if (DatLich.Id_hinhthuc == 1)
                {
                    //DatLich.TrangThaiThanhToan = true; // Gán TrangThai là false
                    return RedirectToAction("Payment", "Home", new { order = DatLich.Id_Phieudat });
                }
                return RedirectToAction("Index", "Home");
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
        public ActionResult Search(string keyword)
        {
            // Tìm kiếm theo từ khóa keyword trong tên nha sĩ
            var nhaSiResults = db.AspNetUsers.Where(u => u.AspNetRoles.Any(r => r.Name == "NhaSi")).Where(n => n.FullName.Contains(keyword)).ToList();

            // Tìm kiếm theo từ khóa keyword trong tên dịch vụ
            var tintucResults = db.TinTucs.Where(d => d.Tieude.Contains(keyword)).ToList();

            // Tạo một ViewModel để chứa kết quả tìm kiếm
            var searchResults = new SearchViewModel
            {
                Keyword = keyword,
                NhaSiResults = nhaSiResults,
                TinTucResults = tintucResults
            };

            // Kiểm tra kết quả tìm kiếm để xác định view cần hiển thị
            if (nhaSiResults.Any() || tintucResults.Any())
            {
                // Gửi kết quả tìm kiếm cho view "SearchResults"
                return View("SearchResults", searchResults);
            }
            else
            {
                // Không có kết quả tìm kiếm, gửi thông báo cho view "NoResults"
                return View("NoResults", searchResults);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview(DanhGia review)
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Get the current user's ID
                string userId = User.Identity.GetUserId(); // Assuming you have User.Identity available

                // Set the user ID in the review object
                review.Id_Benhnhan = userId;
                // Lưu đánh giá và bình luận vào cơ sở dữ liệu
                db.DanhGias.Add(review);
                db.SaveChanges();

                return RedirectToAction("BlogDetail", new { id = review.Id_tintuc });
            }
            else
            {
                // Nếu dữ liệu không hợp lệ, quay lại trang Details với thông tin nha sĩ và các đánh giá và bình luận đã nhập trước đó
                TinTuc tintuc = db.TinTucs.Find(review.Id_tintuc);
                tintuc.DanhGias = db.DanhGias.Where(r => r.Id_tintuc == review.Id_tintuc).ToList();
                return View("BlogDetail", tintuc);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReviewNhaSi(DanhGiaNhaSi reviewnhasi)
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Get the current user's ID
                string userId = User.Identity.GetUserId(); // Assuming you have User.Identity available

                // Set the user ID in the review object
                reviewnhasi.Id_Benhnhan = userId;
                // Lưu đánh giá và bình luận vào cơ sở dữ liệu
                db.DanhGiaNhaSis.Add(reviewnhasi);
                db.SaveChanges();

                return RedirectToAction("DetailsDoctor", new { id = reviewnhasi.Id_Nhasi });
            }
            else
            {
                // Nếu dữ liệu không hợp lệ, quay lại trang Details với thông tin nha sĩ và các đánh giá và bình luận đã nhập trước đó
                AspNetUser nhasi = db.AspNetUsers.Find(reviewnhasi.Id_Nhasi);
                nhasi.DanhGiaNhaSis = db.DanhGiaNhaSis.Where(r => r.Id_Nhasi == reviewnhasi.Id_Nhasi).ToList();
                return View("DetailsDoctor", nhasi);
            }
        }
        [Authorize]
        public ActionResult BookingView()
        {

            // Lấy ID đăng nhập của người dùng hiện tại
            string currentUserId = User.Identity.GetUserId();
            string tennguoidung = User.Identity.GetUserName();
            ViewBag.tennguoidung = tennguoidung;
            // Lấy danh sách lịch hẹn dựa trên ID đăng nhập
            var lichHens = db.PhieuDatLiches
                .Where(l => l.IdBenhNhan == currentUserId).OrderBy(l => l.NgayKham).ToList();
            return View(lichHens);
        }
        [HttpPost]
        public ActionResult CancelBooking(int bookingId)
        {
            // Kiểm tra trạng thái của đặt lịch và thực hiện hủy nếu hợp lệ
            var booking = db.PhieuDatLiches.Find(bookingId);
            if (booking != null && booking.TrangThai == false) // Thay đổi ở đây
            {
                db.PhieuDatLiches.Remove(booking);
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        public ActionResult Payment(int order)
        {
            // Retrieve appointment information based on the provided order ID
            var appointment = db.PhieuDatLiches.Find(order);
         
            if (appointment == null)
            {
                // Handle the case where the appointment is not found
                // You may want to redirect the user to an error page or take appropriate action
                return RedirectToAction("Index", "Home");
            }

            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán đặt lịch";
            string returnUrl = "https://localhost:44374/Home/ConfirmPaymentClient";
            string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = "150000";
            string orderid = DateTime.Now.Ticks.ToString(); //mã đơn hàng
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
    {
        { "partnerCode", partnerCode },
        { "accessKey", accessKey },
        { "requestId", requestId },
        { "amount", amount },
        { "orderId", orderid },
        { "orderInfo", orderInfo },
        { "returnUrl", returnUrl },
        { "notifyUrl", notifyurl },
        { "extraData", extraData },
        { "requestType", "captureMoMoWallet" },
        { "signature", signature }

    };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            // Assuming the payment is successful, update TrangThaiThanhToan to true
            if (jmessage.GetValue("errorCode").ToString() == "0")
            {

                appointment.TrangThaiThanhToan = true;
                db.SaveChanges();
            }

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }


        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult ConfirmPaymentClient(Result result)
        {
            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            return View();
        }

    }
}