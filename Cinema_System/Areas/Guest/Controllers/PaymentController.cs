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
using System.Security.Cryptography;

using Cinema.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Net.Mail;
using System.Net;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc;

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
      
        public PaymentController(PayOSService payOSService, PayOS payOS, ApplicationDbContext context
           )
        {
            _payOSService = payOSService;
            _payOS = payOS;
            _context = context;
           
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            if (request == null || request.TotalAmount <= 0)
            {
                return BadRequest("Invalid payment request.");
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                var newGuestUser = new ApplicationUser // Ensure this matches your user model
                {
                    Id = Guid.NewGuid().ToString(), // Generate a unique ID
                    UserName = "Guest_" + Guid.NewGuid().ToString().Substring(0, 8), // Random username
                    Email = null, // Guest users may not have an email
                    NormalizedUserName = null,
                    NormalizedEmail = null,
                    PhoneNumber = null,
                    EmailConfirmed = false
                };

                _context.Users.Add(newGuestUser);
                await _context.SaveChangesAsync(); // Ensure the user is saved before assigning the ID

                userId = newGuestUser.Id; // Use the new guest user's ID

                // Store in Claims (Optional, so the user is recognized in future orders)
                var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
                var identity = new ClaimsIdentity(claims, "Anonymous");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(principal);
            }
            //=========> tạo id cho người dùng anonymous 

            Coupon coupon = _context.Coupons.FirstOrDefault(c => c.Code == request.Coupon);

            OrderTable order = new OrderTable
            {
                Status = OrderStatus.Pending,
                TotalAmount = request.TotalAmount,
                UserID = userId, // Now userId will never be null
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
                    ProductId = _context.Products.FirstOrDefault(p => p.Name == food.name).ProductID,
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
                // test returnUrl
            
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
                .FirstOrDefaultAsync(o => o.OrderID == orderCode);

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
            

            string secretKey = "h23hriu2ibfas92"; // Store securely in app settings or environment variables
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

            // Define QR Code file path (Temporary location)
            string qrFileName = $"QR_Ticket_{order.OrderID}.png";
            string qrFilePath = Path.Combine(Path.GetTempPath(), qrFileName);

            // Generate QR Code and save to file
            using (QRCodeGenerator codeGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = codeGenerator.CreateQrCode(validationUrl, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCoder = new QRCode(qrCodeData))
                using (Bitmap bitMap = qrCoder.GetGraphic(20))
                {
                    bitMap.Save(qrFilePath, ImageFormat.Png);
                }
            }

            // Email Content
            string emailBody = $@"
        <p>Your ticket has been generated. Please find your QR code attached.</p>
        <p>Scan the QR code to validate your ticket at the venue.</p>
    ";

            // Send Email with Attachment
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential("DE180924ngoanhquan@gmail.com", "uvjs reiv emzl dlsk"); // Replace with your credentials
                client.EnableSsl = true;

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("DE180924ngoanhquan@gmail.com"); // Sender
                    message.To.Add("ngoanhquan0806@gmail.com"); // Recipient
                    message.Subject = "Your Ticket QR Code";
                    message.Body = emailBody;
                    message.IsBodyHtml = true;

                    // Attach QR Code
                    if (System.IO.File.Exists(qrFilePath))
                    {
                        message.Attachments.Add(new Attachment(qrFilePath));
                    }

                    await client.SendMailAsync(message);
                   
                }
            }

            // Clean up: Delete QR file after sending
            if (System.IO.File.Exists(qrFilePath))
            {
                System.IO.File.Delete(qrFilePath);
            }
        }
        //sample url : https://localhost:7115/Staff/Staff/ValidAuthentication?OrderID=3&Key=hcOct9fXJQekxBIFe6Z1awlfk91oRVhS%2Bics1XO8JC8%3D&Timestamp=1742815101

        #region API



        //https://localhost:7115/Guest/Payment/TestSendQR?orderId=3 -> test url
        // Test API to send QR code email   
        [HttpGet]
        public async Task<IActionResult> TestSendQR(int orderId)
        {
            var order = await _context.OrderTables
                .Include(o => o.User) // Ensure User is loaded
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
            {
                return NotFound(new { message = "Order không tồn tại" });
            }

            if (order.User == null)
            {
                return NotFound(new { message = "User không tồn tại trong đơn hàng" });
            }

            await GenerateTicket(order);

            return Ok("QR Code email sent successfully!");
        }



        #endregion


    }
}
