using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Cinema_System.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class StaffController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public StaffController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        [HttpGet]
        [Authorize(Roles = "Staff")] // 🛡️ Only Staff can access this API
        //public async Task<IActionResult> ValidAuthentication(int OrderID, string Key, long Timestamp)
        //{
        //    string secretKey = "h23hriu2ibfas92";
        //    string dataToVerify = $"{OrderID}:{Timestamp}";
        //    Key = WebUtility.UrlDecode(Key);

        //    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
        //    {
        //        string expectedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToVerify)));

        //        // ❌ Reject if Key doesn't match
        //        if (expectedHash != Key)
        //        {
        //            return Unauthorized("Invalid QR Code");
        //        }

        //        // ⏳ Reject if the QR Code is expired (valid for 10 min)
        //        long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        //        if (currentTimestamp - Timestamp > 600) // 600 sec = 10 min
        //        {
        //            return Unauthorized("QR Code Expired");
        //        }
        //        IEnumerable<OrderDetail> order = await _unitOfWork.OrderDetail.GetAllAsync(u => u.OrderID == OrderID,
        //        includeProperties: "Product,ShowtimeSeat.Showtime,ShowtimeSeat.Showtime.Room,ShowtimeSeat.Showtime.Room.Theater,ShowtimeSeat.Showtime.Movie,ShowtimeSeat.Seat,Order.Coupon,Order.User");


        //        return View(order);
        //    }
        //}


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ValidAuthentication(string OrderID, string Key, long Timestamp)
        {
            string secretKey = "h23hriu2ibfas92";
            // Không ép kiểu — dùng nguyên string giống khi tạo token
            string dataToVerify = $"{OrderID}:{Timestamp}";

            //Key = WebUtility.UrlDecode(Key);
            Console.WriteLine($"[VALIDATE] Token received: {Key}");
            Console.WriteLine($"[VALIDATE] DataToVerify: {dataToVerify}");

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                string expectedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToVerify)));
                Console.WriteLine($"[VALIDATE] ExpectedHash recomputed: {expectedHash}");

                if (expectedHash != Key)
                    return Unauthorized("Invalid QR Code");

                long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (currentTimestamp - Timestamp > 600)
                    return Unauthorized("QR Code Expired");

                // Nếu OrderID là số trong DB, convert ở đây:
                // long parsedOrderId = long.Parse(OrderID);
                // var order = await _unitOfWork.OrderDetail.GetAllAsync(u => u.OrderID == parsedOrderId, ...);

                // Nếu OrderID trên DB là string, dùng trực tiếp:
                IEnumerable<OrderDetail> order = await _unitOfWork.OrderDetail.GetAllAsync(
                    u => u.OrderID.ToString() == OrderID,
                    includeProperties: "Product,ShowtimeSeat.Showtime,ShowtimeSeat.Showtime.Room,ShowtimeSeat.Showtime.Room.Theater,ShowtimeSeat.Showtime.Movie,ShowtimeSeat.Seat,Order.Coupon,Order.User"
                );

                return View(order);
            }
        }


        public IActionResult CameraScan()
        {
            return View();
        }
    }
}