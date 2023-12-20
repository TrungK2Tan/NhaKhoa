﻿using Microsoft.AspNet.Identity;
using MoMo;
using Newtonsoft.Json.Linq;
using NhaKhoa.Models;
using NhaKhoa.Other;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NhaKhoa.Controllers
{
    public class BookingDentistController : Controller
    {
        private NhaKhoaModel db = new NhaKhoaModel();
        // GET: BookingDentist
        [Authorize]
        public ActionResult Appointment(DateTime NgayKham, string TenNhaSi,string idnhasi,string Ca)
        {
            ViewBag.NgayKham = NgayKham;
            ViewBag.TenNhaSi = TenNhaSi;
            ViewBag.Idnhasi = idnhasi;
            ViewBag.Ca = Ca;
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
                    return RedirectToAction("Payment", "BookingDentist", new { order = DatLich.Id_Phieudat });
                }
                if (DatLich.Id_hinhthuc == 2)
                {
                    return RedirectToAction("PaymentVNPay", "BookingDentist", new { order = DatLich.Id_Phieudat });
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
                .Select(t => new {
                    IdNhaSi = t.Id_Nhasi,
                    TenNhaSi = t.AspNetUser.FullName,
                    WorkingShifts = db.KhungGios.Where(k => k.Id_khunggio == t.Id_khunggio).Select(k => k.TenCa).ToList()
                })
                .ToList();

            return Json(nhaSiList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Payment(int order)
        {
            // Retrieve appointment information based on the provided order ID
            var appointment = db.PhieuDatLiches.Find(order);
            string gia = appointment.Gia.ToString();
            string order_datlich = order.ToString();
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

            string amount = gia;
            string orderid = order_datlich; //mã đơn hàng
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

        public async Task<ActionResult> PaymentVNPay(int order)
        {
            // Retrieve appointment information based on the provided order ID
            var appointment = db.PhieuDatLiches.Find(order);
            if (appointment == null)
            {
                // Handle the case where the appointment is not found
                // You may want to redirect the user to an error page or take appropriate action
                return RedirectToAction("Index", "Home");
            }
            // Original amount stored in the database
            int originalAmount = 150000;

            // Display amount for payment page (original amount with additional zeros)
            string displayAmount = (originalAmount * 100).ToString();
            string url = ConfigurationManager.AppSettings["vnp_Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", displayAmount); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", order.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            appointment.TrangThaiThanhToan = true;
            db.SaveChanges();
            // Lấy ngày giờ đặt lịch
            DateTime appointmentDate = appointment.NgayKham?.Date ?? DateTime.MinValue;
            //DateTime startTime = appointmentDate + lichHen.GioBatDau.Value;
            //DateTime endTime = appointmentDate + lichHen.GioKetThuc.Value;


            // Nội dung email 
            string emailTo = appointment.AspNetUser.Email; // Email address of the appointment holder
            string subject = "Thông báo đặt lịch hẹn thành công";
            string body = $"Lịch hẹn của bạn đã được chấp nhận thành công.\n" +
                  $"Thông tin lịch hẹn:\n" +
                  $"- Nha sĩ: {appointment.AspNetUser.FullName}\n" +
                  $"- Ngày: {appointmentDate.ToShortDateString()}\n" +
                  //$"- Giờ bắt đầu: {startTime.ToShortTimeString()}\n" +
                  //$"- Giờ kết thúc: {endTime.ToShortTimeString()}" +
                  $"-Bạn vui lòng đến đúng ngay khám nhé! ";

            // Configure SendGrid information
            string sendGridApiKey = "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";
            string sendGridFromEmail = "kakashi252k2@gmail.com";
            string sendGridFromName = "Nha Khoa TDM";

            // Tạo đối tượng SendGridMessage
            SendGridMessage sendGridMessage = new SendGridMessage();
            sendGridMessage.From = new EmailAddress(sendGridFromEmail, sendGridFromName);
            sendGridMessage.AddTo(emailTo);
            sendGridMessage.Subject = subject;
            sendGridMessage.PlainTextContent = body;
            sendGridMessage.HtmlContent = body;

            // Tạo đối tượng SendGridClient và gửi email
            var sendGridClient = new SendGridClient(sendGridApiKey);

            try
            {
                var response = await sendGridClient.SendEmailAsync(sendGridMessage);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu gửi email không thành công
                // ...
            }

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentVNPayConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }
    }
}

