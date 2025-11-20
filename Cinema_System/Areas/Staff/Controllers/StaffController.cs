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
        [AllowAnonymous]
        public async Task<IActionResult> ValidAuthentication(string OrderID, string Key, long Timestamp, bool IsScanned , int count)
        {
            string secretKey = "h23hriu2ibfas92";
            // Không ép kiểu — dùng nguyên string giống khi tạo token
            string dataToVerify = $"{OrderID}:{Timestamp}:{IsScanned}:{count}";

            var orderDetails = await _unitOfWork.OrderDetail.GetAsync(
                      u => u.OrderID.ToString() == OrderID
                  );

            //Key = WebUtility.UrlDecode(Key);
            Console.WriteLine($"[VALIDATE] Token received: {Key}");
            Console.WriteLine($"[VALIDATE] DataToVerify: {dataToVerify}");

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                string expectedHash = Convert.ToBase64String(
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToVerify))
                );

                // 1. Verify hash first
                if (expectedHash != Key)
                    return Unauthorized("Invalid QR Code");

                // 2. Timestamp check
                long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (currentTimestamp - Timestamp > 604800)
                    return Unauthorized("QR Code Expired");

                // 3. Already used?
                if (orderDetails.IsScanned)
                    return Unauthorized("QR Code Already Used");

                // 5. Update scan count
                orderDetails.NumberOfScan -= 1;

                // 6. Mark as used if last scan
                if (orderDetails.NumberOfScan <= 0)
                {
                    orderDetails.IsScanned = true;
                    orderDetails.ScannedAt = DateTime.Now;
                }
                  


                _unitOfWork.OrderDetail.Update(orderDetails);
                await _unitOfWork.SaveAsync();

                // 7. Load order details for view
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