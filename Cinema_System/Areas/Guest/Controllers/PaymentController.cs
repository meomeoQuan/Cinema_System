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
            await _context.SaveChangesAsync();

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
        {
            // Tìm đơn hàng trong database với User
            var order = await _context.OrderTables
                .Include(o => o.User) // Ensure User is loaded

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

            return View();
        }

        {
            // Generate Ticket Validation URL
            string validationUrl = Url.Action("ValidAuthentication", "Staff",
                new { area = "Staff", OrderID = order.OrderID }, Request.Scheme);

            using (QRCodeGenerator codeGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = codeGenerator.CreateQrCode(validationUrl, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCoder = new QRCode(qrCodeData))
                using (Bitmap bitMap = qrCoder.GetGraphic(20))
                {
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

                // Convert QR Code to Base64
                string qrCodeBase64 = Convert.ToBase64String(ms.ToArray());

            //--------------------------------------------------------------------------------------------------------------------------------
            // Email Content
            string emailBody = $@"
            ";

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
