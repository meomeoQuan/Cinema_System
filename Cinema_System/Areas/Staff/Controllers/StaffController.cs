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
        public async Task<IActionResult> ValidAuthentication(string OrderID, string Key, long Timestamp, bool IsScanned)
        {
            string secretKey = "h23hriu2ibfas92";
            // Không ép kiểu — dùng nguyên string giống khi tạo token
            string dataToVerify = $"{OrderID}:{Timestamp}:{IsScanned}";

            var orderDetails = await _unitOfWork.OrderDetail.GetAsync(
                      u => u.OrderID.ToString() == OrderID
                  );
            //Key = WebUtility.UrlDecode(Key);
            Console.WriteLine($"[VALIDATE] Token received: {Key}");
            Console.WriteLine($"[VALIDATE] DataToVerify: {dataToVerify}");

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                string expectedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToVerify)));
                Console.WriteLine($"[VALIDATE] ExpectedHash recomputed: {expectedHash}");

                if(!orderDetails.IsScanned)
                {
                    
                    orderDetails.IsScanned = true;
                    _unitOfWork.OrderDetail.Update(orderDetails);
                    await _unitOfWork.SaveAsync();

                    if (expectedHash != Key)
                        return Unauthorized("Invalid QR Code");

                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    if (currentTimestamp - Timestamp > 604.800) // 7 days in seconds
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

                return Unauthorized("QR code has been scanned");

               
            }
        }


        public IActionResult CameraScan()
        {
            return View();
        }
    }
}