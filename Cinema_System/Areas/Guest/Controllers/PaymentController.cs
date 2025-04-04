using Microsoft.AspNetCore.Mvc;
using Cinema_System.Areas.Service;
using Cinema_System.Areas.Request;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Models;
using SQLitePCL;
using Cinema.DataAccess.Data;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Cinema.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Cinema_System.Areas
{
    [Area("Guest")]
    [Route("Guest/[controller]/[action]")]
    public class PaymentController : Controller
    {
        private readonly PayOSService _payOSService;
        private readonly PayOS _payOS;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        //private readonly UserManager<IdentityUser> _userManager;
        public PaymentController(PayOSService payOSService, PayOS payOS, ApplicationDbContext context,
            IEmailSender emailSender)
        {
            _payOSService = payOSService;
            _payOS = payOS;
            _context = context;
            _emailSender = emailSender;
            //_userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            if (request == null || request.TotalAmount <= 0)
            {
                return BadRequest("Invalid payment request.");
            }

            Coupon coupon = _context.Coupons.FirstOrDefault(c => c.Code == request.Coupon);

            OrderTable order = new OrderTable
            {
                Status = OrderStatus.Pending,
                TotalAmount = request.TotalAmount,
                UserID = "a1234567-b89c-40d4-a123-456789abcdef",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CouponID = coupon != null? coupon.CouponID : null
            };

            _context.OrderTables.Add(order);
            await _context.SaveChangesAsync();

            int orderId = order.OrderID;

            // Chuẩn bị danh sách sản phẩm từ Seats & Foods
            var items = new List<ItemData>();

            // Thêm ghế vào danh sách
            foreach (var seat in request.Seats)
            {
                ShowtimeSeat showtimeSeat = _context.showTimeSeats.Find(seat.showTimeSeatId);
                items.Add(new ItemData($"Seat {seat.nameSeat}", 1, (int)showtimeSeat.Price));
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderID = orderId,
                    ShowtimeSeatID = seat.showTimeSeatId,
                    Quantity = 1,
                    Price = showtimeSeat.Price,
                   
                });
                await _context.SaveChangesAsync();
            }

            // Thêm thức ăn vào danh sách
            foreach (var food in request.Items)
            {
                items.Add(new ItemData(food.name, food.quantity, food.price));
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderID = orderId,
                    ProductID = _context.Products.FirstOrDefault(p => p.Name == food.name).ProductID,
                    Quantity = food.quantity,
                    Price = food.price,
                });
                await _context.SaveChangesAsync();

            }

            var couponPrice = 0;

            if (coupon != null)
            {
                couponPrice -= (int)(request.TotalAmount * coupon.DiscountPercentage);
                items.Add(new ItemData(coupon.Code, 1, couponPrice));
            }
            // Gọi dịch vụ PayOS để tạo thanh toán
            var response = await _payOSService.CreatePaymentAsync(request.TotalAmount + couponPrice, orderId, items, _payOS);

            if (response.error == 0)
            {
                return Json(new { paymentUrl = ((CreatePaymentResult)response.data).checkoutUrl });
            }

            return BadRequest("Payment failed.");
        }


        // Trang hủy
        [HttpGet]
        public IActionResult CancelUrl(int orderCode)
        {
            var order = _context.OrderTables.FirstOrDefault(o => o.OrderID == orderCode);

            if (order == null)
            {
                return NotFound(new { message = "Order không tồn tại" });
            }

            // Cập nhật trạng thái đơn hàng thành "Canceled"
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ReturnUrl(int orderCode)
        {
            // Tìm đơn hàng trong database với User
            var order = await _context.OrderTables
                .Include(o => o.User) // Ensure User is loaded
                .FirstOrDefaultAsync(o => o.OrderID == 2);

            if (order == null)
            {
                return NotFound(new { message = "Order không tồn tại" });
            }

            if (order.User == null)
            {
                return NotFound(new { message = "User không tồn tại trong đơn hàng" });
            }

          

            // Cập nhật trạng thái đơn hàng thành "Completed"
            order.Status = OrderStatus.Completed;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(); // Ensure async save



            // Gửi QR code qua email
            await GenerateTicket(order);

            return View();
        }

        public async Task GenerateTicket(OrderTable order)
        {



            string secretKey = "h23hriu2ibfas92"; // Store securely in app settings or environment variables optional
            string orderId = order.OrderID.ToString();
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string ? validationUrl = "";
            // 🔐 Generate HMAC-SHA256 token
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                string dataToSign = $"{orderId}:{timestamp}"; // OrderID + Timestamp
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                string token = Convert.ToBase64String(hash); // Encode as Base64

                // 🏷️ Generate the Secure Validation URL
                 validationUrl = Url.Action("ValidAuthentication", "Staff",
                    new { area = "Staff", OrderID = orderId, Key = token, Timestamp = timestamp }, Request.Scheme);
            }


        https://localhost:7251/Staff/Staff/ValidAuthentication?ticketId=ds#Staff
            // Generate QR Code
            using (MemoryStream ms = new MemoryStream())
            using (QRCodeGenerator codeGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = codeGenerator.CreateQrCode(validationUrl, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCoder = new QRCode(qrCodeData))
                using (Bitmap bitMap = qrCoder.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                }

                // Convert QR Code to Base64
                string qrCodeBase64 = Convert.ToBase64String(ms.ToArray());

                // Email Content
                string emailBody = $@"
            <p>Your ticket has been generated. Please show the QR code below when entering the venue.</p>
            <p>Scan this QR code to validate your ticket:</p>
            <img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code' />
        ";

                // Send Email
                //order.User.Email
                await _emailSender.SendEmailAsync("ngoanhquan0806@gmail.com", "Your Ticket QR Code", emailBody);


            }
        }




    }
}
