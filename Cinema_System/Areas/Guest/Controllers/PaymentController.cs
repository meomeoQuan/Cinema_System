using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Cinema_System.Areas.Request;
using Cinema_System.Areas.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using QRCoder;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
        private readonly UserManager<IdentityUser> _userManager;
        public PaymentController(PayOSService payOSService, PayOS payOS, ApplicationDbContext context,
            IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _payOSService = payOSService;
            _payOS = payOS;
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> PaymentSummary([FromBody]PaymentRequest request)
        {
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(PaymentRequest request)
        {
            if (request == null || request.TotalAmount <= 0)
            {
                return BadRequest("Invalid payment request.");
            }

            // 🔍 Find coupon
            var coupon = _context.Coupons.FirstOrDefault(c => c.Code == request.Coupon);

            // 👤 Check if user exists by email
            var user = await _userManager.FindByEmailAsync(request.Email);

            // 🆕 Create if not exists
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    Role = SD.Role_Guest,
                };
                var createResult = await _userManager.CreateAsync(user, "Default@123");

                if (!createResult.Succeeded)
                {
                    return BadRequest("Failed to create user account.");
                }
            }

            // 🧾 Create new order
            var order = new OrderTable
            {
                Status = OrderStatus.Pending,
                TotalAmount = request.TotalAmount,
                UserID = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CouponID = coupon?.CouponID
            };

            _context.OrderTables.Add(order);
            await _context.SaveChangesAsync(); // get order ID

            int orderId = order.OrderID;
            var items = new List<ItemData>();
            var orderDetails = new List<OrderDetail>();

            // 🎟️ Add seats
            foreach (var seat in request.Seats)
            {
                var showtimeSeat = await _context.showTimeSeats.FindAsync(seat.showTimeSeatId);
                if (showtimeSeat != null)
                {
                    items.Add(new ItemData($"Seat {seat.nameSeat}", 1, (int)showtimeSeat.Price));
                    orderDetails.Add(new OrderDetail
                    {
                        OrderID = orderId,
                        ShowtimeSeatID = seat.showTimeSeatId,
                        Quantity = 1,
                        Price = showtimeSeat.Price
                    });
                }
            }

            // 🍿 Add food
            foreach (var food in request.Items)
            {
                var product = _context.Products.FirstOrDefault(p => p.Name == food.name);
                if (product != null)
                {
                    items.Add(new ItemData(food.name, food.quantity, food.price));
                    orderDetails.Add(new OrderDetail
                    {
                        OrderID = orderId,
                        ProductID = product.ProductID,
                        Quantity = food.quantity,
                        Price = food.price
                    });
                }
            }

            // Save all details once
            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();

            // 💸 Coupon discount
            int discount = 0;
            if (coupon != null)
            {
                discount = (int)(request.TotalAmount * coupon.DiscountPercentage / 100);
                items.Add(new ItemData($"Coupon {coupon.Code}", 1, -discount));
            }

            int finalAmount = request.TotalAmount - discount;

            // 🔗 Call PayOS
            var response = await _payOSService.CreatePaymentAsync(finalAmount, orderId, items, _payOS);

            if (response.error == 0)
            {
                var paymentResult = (CreatePaymentResult)response.data;
                return Json(new { paymentUrl = paymentResult.checkoutUrl });
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
            string? validationUrl = "";
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