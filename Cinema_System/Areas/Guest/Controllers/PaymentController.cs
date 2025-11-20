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
using static iTextSharp.text.pdf.AcroFields;

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
            IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            _payOSService = payOSService;
            _payOS = payOS;
            _context = context;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
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

            if (userId == null && request.Guest != null)
            {
                var newGuestUser = new ApplicationUser // Ensure this matches your user model
                {
                    Id = Guid.NewGuid().ToString(), // Generate a unique ID
                    UserName = "Guest_" + Guid.NewGuid().ToString().Substring(0, 8), // Random username
                    Email = request.Guest.email, // Guest users may not have an email
                    NormalizedUserName = null,
                    NormalizedEmail = null,
                    PhoneNumber = request.Guest.phone,
                    FullName = request.Guest.fullname,
                    EmailConfirmed = false,
                    IsAnonymous = true,
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
            long orderCode = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

            OrderTable order = new OrderTable
            {
                OrderID = orderCode,
                Status = OrderStatus.Pending,
                TotalAmount = request.TotalAmount,
                UserID = userId, // Now userId will never be null
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CouponID = coupon != null ? coupon.CouponID : null
            };

            _context.OrderTables.Add(order);
            //await _context.SaveChangesAsync();

            long orderId = order.OrderID;

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
                //await _context.SaveChangesAsync();
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
                //await _context.SaveChangesAsync();

            }

            var couponPrice = 0;

            if (coupon != null)
            {
                couponPrice -= (int)(request.TotalAmount * coupon.DiscountPercentage);
                items.Add(new ItemData(coupon.Code, 1, couponPrice));
            }

            // === TẠO DANH SÁCH EMAIL THEO THỨ TỰ ===
            var emailList = new List<string>();

            if (!string.IsNullOrWhiteSpace(request.Guest?.email))
                emailList.Add(request.Guest.email.Trim());

            if (request.FriendEmails?.Emails != null)
            {
                foreach (var e in request.FriendEmails.Emails)
                {
                    if (!string.IsNullOrWhiteSpace(e) && !emailList.Contains(e.Trim(), StringComparer.OrdinalIgnoreCase))
                        emailList.Add(e.Trim());
                }
            }

            order.RecipientEmails = string.Join(",", emailList); // ← QUAN TRỌNG
            _context.OrderTables.Add(order);
            await _context.SaveChangesAsync();
            // ========================================

            // Gọi dịch vụ PayOS để tạo thanh toán
            var response = await _payOSService.CreatePaymentAsync(request.TotalAmount + couponPrice, orderId, items, _payOS);

            if (response.error == 0)
            {
                // test returnUrl

                return Json(new { paymentUrl = ((CreatePaymentResult)response.data).checkoutUrl });
            }

            return BadRequest("Payment failed.");
        }

        [HttpPost("/products")]
        public async Task<IActionResult> CreatePaymentProduct([FromBody] PaymentRequest request)
        {
            var items = new List<ItemData>();
            var couponPrice = 0;
            Coupon coupon = _context.Coupons.FirstOrDefault(c => c.Code == request.Coupon);
            OrderTable orderTable = _context.OrderTables.FirstOrDefault(o => o.OrderID == request.OrderCode);

            foreach (var food in request.Items)
            {
                items.Add(new ItemData(food.name, food.quantity, food.price));

            }

            if (orderTable == null)
            {
                return BadRequest("Payment failed.");
            }

            if (coupon != null)
            {
                couponPrice -= (int)(request.TotalAmount * coupon.DiscountPercentage);
                items.Add(new ItemData(coupon.Code, 1, couponPrice));
            }

            var response = await _payOSService.CreatePaymentAsync(request.TotalAmount + couponPrice, orderTable.OrderID, items, _payOS);

            if (response.error == 0)
            {
                // test returnUrl

                return Json(new { paymentUrl = ((CreatePaymentResult)response.data).checkoutUrl });
            }

            return BadRequest("Payment failed.");
        }


        // Trang hủy
        [HttpGet]
        public IActionResult CancelUrl(long orderCode)
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
        public async Task<IActionResult> ReturnUrl(long orderCode)
        {
            var order = await _context.OrderTables
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderID == orderCode);

            if (order == null)
                return NotFound(new { message = "Order không tồn tại" });

            // === LẤY DANH SÁCH EMAIL ĐÃ LƯU ===
            var emailList = string.IsNullOrWhiteSpace(order.RecipientEmails)
                ? new List<string>()
                : order.RecipientEmails.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(e => e.Trim())
                     .ToList();

            // Nếu không có email nào (trường hợp lỗi), ít nhất gửi cho User chính
            if (!emailList.Any() && order.User?.Email != null)
                emailList.Add(order.User.Email);

            // Cập nhật trạng thái
            order.Status = OrderStatus.Completed;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            int count = emailList.Count;

            // === GỬI EMAIL CHO TẤT CẢ NGƯỜI TRONG DANH SÁCH ===
            if (emailList.Any())
            {
             
                        foreach (var email in emailList)
                        {
                            await GenerateTicket(order, email, count); // hàm cũ của bạn vẫn dùng được
                        }
                 
            }

                return View(); // hoặc RedirectToAction("Success")
        }

        public async Task GenerateTicket(OrderTable order, string emailUser, int count)
        {


            string secretKey = "h23hriu2ibfas92"; // Store securely in app settings or environment variables optional
            string orderId = order.OrderID.ToString();
            bool IsScanned = false;
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            var orderDetail = await _unitOfWork.OrderDetail.GetAsync(
                      u => u.OrderID == order.OrderID
                  );

            if(orderDetail!= null)
            {
                orderDetail.NumberOfScan = count;
                orderDetail.IsScanned = false;
                _unitOfWork.OrderDetail.Update(orderDetail);
                await _unitOfWork.SaveAsync();
            }
               

            string? validationUrl = "";
            // 🔐 Generate HMAC-SHA256 token
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                string dataToSign = $"{orderId}:{timestamp}:{IsScanned}:{count}"; // OrderID + Timestamp
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                string token = Convert.ToBase64String(hash);// Encode as Base64

                // 🏷️ Generate the Secure Validation URL
                validationUrl = Url.Action("ValidAuthentication", "Staff",
                   new { area = "Staff", OrderID = orderId, Key = token, Timestamp = timestamp, IsScanned = IsScanned , count = count}, Request.Scheme);
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
            //------------------------------------------------------------------Ticket content-------------------------------------------------------------
            var text = await _unitOfWork.OrderDetail.GetAllAsync();
            Console.WriteLine("Debug Info: Fetched OrderDetails Count = " + text.Count());

            IEnumerable<OrderDetail> orderDetails = await _unitOfWork.OrderDetail.GetAllAsync(
                       u => u.OrderID == order.OrderID,
                       includeProperties: "Product,ShowtimeSeat.Showtime,ShowtimeSeat.Showtime.Room,ShowtimeSeat.Showtime.Room.Theater,ShowtimeSeat.Showtime.Movie,ShowtimeSeat.Seat,Order.Coupon,Order.User"
                   );

     
            // Pull base data
            var first = orderDetails.First();
            string cinemaName = first.ShowtimeSeat.Showtime.Room.Theater.Name;
            string roomName = first.ShowtimeSeat.Showtime.Room.RoomNumber;
            string movieName = first.ShowtimeSeat.Showtime.Movie.Title;
            int movieDuration = first.ShowtimeSeat.Showtime.Movie.Duration;
            DateOnly showDate = first.ShowtimeSeat.Showtime.ShowDate;

            // Format as dd:MM:yyyy
            string formattedDate = showDate.ToString("dd/MM/yyyy");

            TimeSpan showTime = first.ShowtimeSeat.Showtime.ShowTimes;

            // Convert TimeSpan → TimeOnly
            TimeOnly timeOnly = TimeOnly.FromTimeSpan(showTime);

            // Combine DateOnly + TimeOnly → DateTime
            DateTime showDateTime = showDate.ToDateTime(timeOnly);

            // Format
            string showtimeStr = showDateTime.ToString("HH:mm");




            // Seats
            var seatList = orderDetails
                .Where(o => o.ShowtimeSeat != null)
                .Select(o => o.ShowtimeSeat.Seat.SeatName)
                .Distinct()
                .ToList();

            // Products (popcorn, drinks, addons, etc.)
            var productList = orderDetails
                .Where(o => o.Product != null)
                .Select(o => $"{o.Product.Name} x{o.Quantity}")
                .ToList();

            // Build HTML parts
            string seatsHtml = string.Join(", ", seatList);
            string productsHtml = productList.Count > 0
                ? string.Join("<br>", productList)
                : "No additional products";


            //--------------------------------------------------------------------------------------------------------------------------------
            // Email Content
            string emailBody = $@"
                <h2>Your Ticket Details</h2>

                <p>Hey! Your ticket is ready. Please Check all your info below.</p>
                 
                <h4>Order ID: {order.OrderID}</h4>                    

                <h3>🎬 Movie Info</h3>
                <p><strong>Movie:</strong> {movieName}</p>
                <p><strong>Duration:</strong> {movieDuration} minutes</p>

                <h3>🏢 Cinema</h3>
                <p><strong>Cinema:</strong> {cinemaName}</p>
                <p><strong>Room:</strong> {roomName}</p>

                <h3>📅 Show Date</h3>
                <p>{formattedDate}</p>

                <h3>🕒 Showtime</h3>
                <p>{showtimeStr}</p>

                <h3>💺 Seat(s)</h3>
                <p>{seatsHtml}</p>

                <h3>🍿 Products</h3>
                <p>{productsHtml}</p>

                <br>

                <p>Your QR code is attached. Scan it at the entrance to validate your ticket.</p>
            ";

            // Send Email with Attachment
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.Credentials = new NetworkCredential("DE180924ngoanhquan@gmail.com", "uvjs reiv emzl dlsk"); // Replace with your credentials
                client.EnableSsl = true;

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress("DE180924ngoanhquan@gmail.com"); // Sender
                    message.To.Add(emailUser); // Recipient
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

            //// Clean up: Delete QR file after sending
            //if (System.IO.File.Exists(qrFilePath))
            //{
            //    System.IO.File.Delete(qrFilePath);
            //}
        }
        //sample url : https://localhost:7115/Staff/Staff/ValidAuthentication?OrderID=3&Key=hcOct9fXJQekxBIFe6Z1awlfk91oRVhS%2Bics1XO8JC8%3D&Timestamp=1742815101

        #region API



      


        #endregion


    }
}
