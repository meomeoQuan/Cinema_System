using System.Security.Cryptography;
using System.Text;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> ValidAuthentication(int OrderID, string Key, long Timestamp)
        {
            string secretKey = "h23hriu2ibfas92";
            string dataToVerify = $"{OrderID}:{Timestamp}";

           
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                string expectedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToVerify)));

                // ❌ Reject if Key doesn't match
                if (expectedHash != Key)
                {
                    return Unauthorized("Invalid QR Code");
                }

                //// ⏳ Reject if the QR Code is expired (valid for 10 min)
                //long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                //if (currentTimestamp - Timestamp > 600) // 600 sec = 10 min
                //{
                //    return Unauthorized("QR Code Expired");
                //}
                IEnumerable<OrderDetail> order = await _unitOfWork.OrderDetail.GetAllAsync(u => u.OrderID == OrderID,
                includeProperties: "User,Product,ShowtimeSeat.Showtime,ShowtimeSeat.Showtime.Room,ShowtimeSeat.Showtime.Room.Cinema,ShowtimeSeat.Showtime.Movie,ShowtimeSeat.Seat,Order.Coupon,Order");

                // i want to take (Product): Name, Price, Quantity . (ShowtimeSeat.Showtime) : ShowDate (DateTime -> string). (ShowtimeSeat.Showtime.Movie) : Title, Genre, Duration(int -> string ) (ShowtimeSeat.Showtime.Room) : RoomNumber
                // ShowtimeSeat.Showtime.Room.Cinema -> Name, Address

                // ✅ QR Code is valid, Staff can check the ticket
                return View(order);
            }
        }



        public IActionResult CameraScan()
        {
            return View();
        }
    }
}
