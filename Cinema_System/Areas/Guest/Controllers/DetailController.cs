using Cinema.Models.ViewModels;
using Cinema.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Cinema.DataAccess.Repository.IRepository;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Cinema_System.Models;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class DetailController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]

        private  MovieDetailVM movieDetail { get; set; }

        public DetailController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int? MovieID)
        {
          
            MovieDetailVM? Movie = new MovieDetailVM()
            {
                Movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID),
                OrderDetail = new OrderDetail()


            };
            ViewData["MovieID"] = MovieID; // Store MovieID using ViewData
           
            return View(Movie);
        }

        //C1
        public async Task<IActionResult> Details(int MovieID, string? targetDate = null, string? targetCity = null, string? targetTime = null)
        {
            // ✅ Get User ID from Claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // ✅ Fetch Movie Details
            var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID);
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }

            // ✅ Fetch Showtimes for the selected movie
            var showTimes = await _unitOfWork.showTime.GetAllAsync(
                u => u.MovieID == MovieID, includeProperties: "Cinema,Room"
            );

            // ✅ Fetch Showtime Seats
            var showtimeSeats = await _unitOfWork.ShowTimeSeat.GetAllAsync(
                includeProperties: "Seat"
            );

            // ✅ Get available cities
            var availableCities = showTimes
                .Select(show => show.Cinema.CinemaCity)
                .Distinct()
                .Select(city => new
                {
                    City = city,
                    Selected = (!string.IsNullOrEmpty(targetCity) && city.Equals(targetCity, StringComparison.OrdinalIgnoreCase))
                })
                .ToList();

            // ✅ Get available dates
            var availableDates = showTimes
                .Select(show => show.ShowDates)
                .Distinct()
                .Select(date => new
                {
                    Date = date,
                    Selected = (!string.IsNullOrEmpty(targetDate) && date == targetDate)
                })
                .ToList();

            // ✅ Get all showtimes (No filtering here, let JS filter)
            var showtimeList = showTimes.Select(show => new
            {
                date = show.ShowDates,
                CinemaName = show.Cinema.Name,
                CinemaId = show.CinemaID,
                CinemaAddress = show.Cinema.Address,
                City = show.Cinema.CinemaCity,
                RoomId = show.Room.RoomID,
                RoomName = show.Room.RoomNumber,
                Showtime = show.ShowTimes,

                Tickets = showtimeSeats
                    .Where(seat => seat.ShowtimeID == show.ShowTimeID)
                    .Select(seat => new
                    {
                        SeatId = seat.SeatID,
                        SeatNumber = seat.Seat.SeatName,

                        SeatType = seat.SeatType.ToString(),

                        Price = seat.Price
                    })
                    .ToList()

            }).ToList();


            // ✅ Get Food Items (No filtering)
            var foodItemsList = await _unitOfWork.Product.GetAllAsync();
            //var selectedFoodItems = foodItems
            //    .Take(1) // Dummy selection logic
            //    .Select(food => new
            //    {
            //        FoodId = food.ProductID,
            //        FoodName = food.Name,
            //        Quantity = 2,
            //        Price = food.Price
            //    })
            //    .ToList();

            // ✅ Calculate Total Price
            //double totalPrice = showtimeList.Sum(st => st.Tickets.Sum(t => t.Price)) + selectedFoodItems.Sum(f => f.Price * f.Quantity);

            // ✅ Return structured response
            var result = new
            {
                UserId = userId,
                MovieId = movie.MovieID,
                MovieName = movie.Title,
                AvailableDates = availableDates, // Now an array of { Date, Selected }
                SelectedDate = targetDate, // Can be removed if frontend uses `AvailableDates`
                AvailableCities = availableCities, // Now an array of { City, Selected }
                City = targetCity, // Can be removed if frontend uses `AvailableCities`
                Showtimes = showtimeList,
                FoodItems = foodItemsList,
                Selected = false,
                TotalPrice = 0
            };

            return Json(new { message = "Success", data = result });
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ConfirmBooking(OrderDetail orderDetail)
        {
            if (orderDetail == null)
            {
                return BadRequest("Order details are missing.");
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            orderDetail.UserId = userId;

            // Deserialize JSON fields
            orderDetail.Tickets = string.IsNullOrEmpty(orderDetail.TicketsJson)
                ? new List<TicketSelectionVM>()
                : JsonConvert.DeserializeObject<List<TicketSelectionVM>>(orderDetail.TicketsJson);

            orderDetail.SelectedSeats = string.IsNullOrEmpty(orderDetail.SelectedSeatsJson)
                ? new List<ShowtimeSeat>()
                : JsonConvert.DeserializeObject<List<ShowtimeSeat>>(orderDetail.SelectedSeatsJson);

            orderDetail.FoodItems = string.IsNullOrEmpty(orderDetail.ItemsJson)
                ? new List<FoodSelectionVM>()
                : JsonConvert.DeserializeObject<List<FoodSelectionVM>>(orderDetail.ItemsJson);

            // Save to database
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.SaveAsync();

            return RedirectToAction("Confirmation", new { orderId = orderDetail.OrderDetailID });
        }






        [HttpPost]
        public IActionResult GenerateTicket(string ticketId)
        {
            string validationUrl = Url.Action("ValidAuthentication", "Home", new { ticketId = ticketId }, Request.Scheme);
            //string validationUrl = "https://localhost:7251/Home/ValidAuthentication";

            //string validationUrl = "https://www.facebook.com";
            //string validationUrl = "https://httpbin.org/anything";
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator codeGenerator = new QRCodeGenerator();
                QRCodeData qRCodeData = codeGenerator.CreateQrCode(validationUrl, QRCodeGenerator.ECCLevel.Q);
                QRCode qRCoder = new QRCode(qRCodeData);

                using (Bitmap bitMap = qRCoder.GetGraphic(20))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

            ViewBag.TicketId = ticketId;
            return View("TestQR");
        }
        //https://localhost:7115/Guest/Detail/GenerateTicket to test QRcode
        // enter staff login to use scan camera Staff account in DBInitializer username is account,,Staff@123 password
        public IActionResult TestQR()
        {
            return View();
        }
        public IActionResult ValidAuthentication(string ticketId)
        {
            if (ValidateTicket(ticketId))
            {
                ViewBag.Message = "Ticket is valid!";
            }
            else
            {
                ViewBag.Message = "Invalid or expired ticket.";
            }

            ViewBag.TicketId = ticketId;
            return View();
        }

        private bool ValidateTicket(string ticketId)
        {
            // Mock validation logic; replace with actual validation (e.g., check database)
            return !string.IsNullOrEmpty(ticketId) && ticketId.StartsWith("TICKET");
        }


    }
}
