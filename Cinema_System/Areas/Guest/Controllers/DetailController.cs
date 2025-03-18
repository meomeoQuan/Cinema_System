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
using System.Net.Sockets;
using Microsoft.CodeAnalysis;

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

        public async Task<IActionResult> Index(int? movieId, int? showtimeId)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = claimsIdentity.FindFirst(ClaimTypes.Name).Subject;

            if (movieId == null)
            {
                return BadRequest("Movie ID is required.");
            }

            var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == movieId);

            if (movie == null)
            {
                return NotFound("Movie not found.");
            }

            List<ShowtimeSeat> showtimeSeats = new();
            if (showtimeId != null)
            {
                showtimeSeats = (await _unitOfWork.ShowTimeSeat.GetAllAsync(
                    s => s.ShowtimeID == showtimeId,
                    includeProperties: "Seat"
                )).ToList();
            }
            List<Product> productList = (List<Product>) await _unitOfWork.Product.GetAllAsync();

            ViewData["MovieID"] = movieId; // Store MovieID using ViewData
            TempData["ShowtimeID"] = showtimeId;
            TempData["UserName"] = userName;
            var movieDetailVM = new MovieDetailVM
            {
                Movie = movie,
                ListCart =  _unitOfWork.ShoppingCart.GetAll(u => u.UserID == userId,includeProperties: "Product,ShowtimeSeat.Seat,User").ToList(), // product of person not system
                OrderTable = new OrderTable(),
                ShowtimeSeats = showtimeSeats,
                products = productList,
               
            };

            //// If AJAX request, return partial view
            //if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    return PartialView("_SeatsPartial", movieDetailVM);
            //}

            return View(movieDetailVM);
        }

        public async Task<IActionResult> SeatChosing(int? seatId, int? movieId, int? showtimeId)
        {
            if (seatId == null || movieId == null)
            {
                return BadRequest("Invalid seat or movie selection.");
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Retrieve movie details
            var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == movieId);
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }

            // Retrieve selected seat details
            var selectedSeat = await _unitOfWork.ShowTimeSeat.GetAsync(
                s => s.ShowtimeSeatID == seatId && s.Showtime.ShowTimeID == showtimeId,
                includeProperties: "Seat"
            );

            if (selectedSeat == null || selectedSeat.Seat == null)
            {
                return NotFound("Seat not found.");
            }

            // Check if the seat is already booked
            if (selectedSeat.Status == ShowtimeSeatStatus.Booked)
            {
                return BadRequest("This seat is already booked.");
            }

            // Mark the seat as booked
            selectedSeat.Status = ShowtimeSeatStatus.Booked;
            _unitOfWork.ShowTimeSeat.Update(selectedSeat);
            await _unitOfWork.SaveAsync();

            // Add seat to shopping cart
            var shoppingCartItem = new ShoppingCart()
            {
                UserID = userId,
                ShowtimeSeatID = seatId,
                Quantity = 1,
                Price = selectedSeat.Price
            };

            _unitOfWork.ShoppingCart.Add(shoppingCartItem);
            await _unitOfWork.SaveAsync();

            // Retrieve all showtime seats
            var showtimeSeats = (await _unitOfWork.ShowTimeSeat.GetAllAsync(
                s => s.ShowtimeID == showtimeId,
                includeProperties: "Seat"
            )).ToList();

            // Retrieve updated shopping cart
            var shoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                u => u.UserID == userId, includeProperties: "Product,ShowtimeSeat"
            ).ToList();

            List<Product> productList = (List<Product>)await _unitOfWork.Product.GetAllAsync();
            // Prepare MovieDetailVM
            var movieDetailVM = new MovieDetailVM()
            {
                Movie = movie,
                ListCart = shoppingCartList,
                OrderTable = new OrderTable(),
                ShowtimeSeats = showtimeSeats,
                products = productList,
               
            };

            ViewData["MovieID"] = movieId; // Store MovieID using ViewData
            TempData["ShowtimeID"] = showtimeId;
            TempData["UserID"] = userId;
            return View("Index", movieDetailVM);
        }


        public async Task<IActionResult> IncreaseQuantity(int productId, int? movieId, int? showtimeId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      

            // Retrieve movie details
            var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == movieId);
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }

            // Retrieve or create a shopping cart item
            var cartItem = await _unitOfWork.ShoppingCart.GetAsync(c => c.UserID == userId && c.ProductID == productId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCart
                {
                    UserID = userId,
                    ProductID = productId,
                    Quantity = 1
                };
                _unitOfWork.ShoppingCart.Add(cartItem);
            }
            else
            {
               
                cartItem.Quantity += 1;
                _unitOfWork.ShoppingCart.Update(cartItem);
            }

            // Retrieve user's shopping cart items
            var shoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                u => u.UserID == userId,
                includeProperties: "Product,ShowtimeSeat"
            ).ToList();

            // Retrieve showtime seats
            var showtimeSeats = (await _unitOfWork.ShowTimeSeat.GetAllAsync(
                s => s.ShowtimeID == showtimeId,
                includeProperties: "Seat"
            )).ToList();

            // Retrieve all products
            var productList = await _unitOfWork.Product.GetAllAsync();

            var movieDetailVM = new MovieDetailVM
            {
                Movie = movie,
                ListCart = shoppingCartList,
                OrderTable = new OrderTable(),
                ShowtimeSeats = showtimeSeats,
                products = productList.ToList()
            };

            await _unitOfWork.SaveAsync();
            ViewData["MovieID"] = movieId; // Store MovieID using ViewData
            TempData["ShowtimeID"] = showtimeId;
            TempData["UserID"] = userId;
            return View("Index", movieDetailVM);
        }

        public async Task<IActionResult> DecreaseQuantity(int productId, int? movieId, int? showtimeId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

          

            // Retrieve the cart item
            var cartItem = await _unitOfWork.ShoppingCart.GetAsync(c => c.UserID == userId && c.ProductID == productId);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                   
                    cartItem.Quantity -= 1;
                    _unitOfWork.ShoppingCart.Update(cartItem);

                }
                else
                {
                    _unitOfWork.ShoppingCart.Remove(cartItem); // Remove item if quantity reaches 0
                }
            }

            // Retrieve movie details
            var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == movieId);
            if (movie == null)
            {
                return NotFound("Movie not found.");
            }


            // Retrieve updated shopping cart
            var shoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                u => u.UserID == userId, includeProperties: "Product,ShowtimeSeat"
            ).ToList();

            // Retrieve all showtime seats
            var showtimeSeats = (await _unitOfWork.ShowTimeSeat.GetAllAsync(
                s => s.ShowtimeID == showtimeId,
                includeProperties: "Seat"
            )).ToList();

            List<Product> productList = (List<Product>)await _unitOfWork.Product.GetAllAsync();

            var movieDetailVM = new MovieDetailVM()
            {
                Movie = movie,
                ListCart = shoppingCartList,
                OrderTable = new OrderTable(),
                ShowtimeSeats = showtimeSeats,
                products = productList,
            };

            ViewData["MovieID"] = movieId; // Store MovieID using ViewData
            TempData["ShowtimeID"] = showtimeId;
            TempData["UserID"] = userId;
            await _unitOfWork.SaveAsync();
            return View("Index", movieDetailVM); // Redirect back to the shopping cart page
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
                ShowtimeId = show.ShowTimeID,
                date = show.ShowDates,
                CinemaName = show.Cinema.Name,
                CinemaId = show.CinemaID,
                CinemaAddress = show.Cinema.Address,
                City = show.Cinema.CinemaCity,
                RoomId = show.Room.RoomID,
                RoomName = show.Room.RoomNumber,
                Showtime = show.ShowTimes,
                SeatList = showtimeSeats
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
//public async Task<IActionResult> Index(int? movieId)
//{
//    if (movieId == null)
//    {
//        return BadRequest("Movie ID is required.");
//    }

//    var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == movieId, includeProperties: "Showtimes");

//    if (movie == null)
//    {
//        return NotFound("Movie not found.");
//    }

//    var movieDetailVM = new MovieDetailVM
//    {
//        Movie = movie,
//        OrderDetail = new OrderDetail(),
//        OrderTable = new OrderTable(),
//        ShowtimeSeats = new List<ShowtimeSeat>()
//    };

//    ViewData["MovieID"] = movieId; // Store MovieID

//    return View(movieDetailVM);
//}
//[HttpGet]
//public async Task<IActionResult> GetSeatsPartial(int? showtimeId)
//{
//    if (showtimeId == null)
//    {
//        return BadRequest("Showtime ID is required.");
//    }

//    var showtimeSeats = await _unitOfWork.ShowTimeSeat.GetAllAsync(
//        s => s.ShowtimeID == showtimeId,
//        includeProperties: "Seat"
//    );

//    if (showtimeSeats == null || !showtimeSeats.Any())
//    {
//        return NotFound("No seats found for this showtime.");
//    }

//    return PartialView("_SeatsPartial", showtimeSeats);
//}

//public async Task<IActionResult> Details(int MovieID, string? targetDate = null, string? targetCity = "Danang", string? targetTime = null)
//{
//    // ✅ Get User ID from Claims
//    var claimsIdentity = (ClaimsIdentity)User.Identity;
//    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//    // ✅ Fetch Movie Details
//    var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID);
//    if (movie == null)
//    {
//        return NotFound("Movie not found.");
//    }

//    // ✅ Fetch Showtimes for the selected movie
//    var showTimes = await _unitOfWork.showTime.GetAllAsync(
//        u => u.MovieID == MovieID, includeProperties: "Cinema,Room"
//    );

//    // ✅ Fetch Showtime Seats
//    var showtimeSeats = await _unitOfWork.ShowTimeSeat.GetAllAsync(
//        includeProperties: "Seat"
//    );

//    // ✅ Get available cities
//    var availableCities = showTimes
//        .Select(show => show.Cinema.CinemaCity)
//        .Distinct()
//        .Select(city => new CityVM
//        {
//            City = city,
//            Selected = (!string.IsNullOrEmpty(targetCity) && city.Equals(targetCity, StringComparison.OrdinalIgnoreCase))
//        })
//        .ToList();

//    // ✅ Get available dates
//    var availableDates = showTimes
//        .Select(show => show.ShowDates)
//        .Distinct()
//        .Select(date => new ShowDateVM
//        {
//            Date = date,
//            Selected = (!string.IsNullOrEmpty(targetDate) && date == targetDate)
//        })
//        .ToList();

//    // ✅ Get all showtimes
//    var showtimeList = showTimes.Select(show => new ShowtimeVM
//    {
//        Date = show.ShowDates,
//        CinemaName = show.Cinema.Name,
//        CinemaId = show.CinemaID,
//        CinemaAddress = show.Cinema.Address,
//        City = show.Cinema.CinemaCity,
//        RoomId = show.Room.RoomID,
//        RoomName = show.Room.RoomNumber,
//        Showtime = show.ShowTimes,

//        SeatList = showtimeSeats
//            .Where(seat => seat.ShowtimeID == show.ShowTimeID)
//            .Select(seat => new SeatVM
//            {
//                SeatId = seat.SeatID,
//                SeatNumber = seat.Seat.SeatName,
//                SeatType = seat.SeatType.ToString(),
//                Price = seat.Price
//            })
//            .ToList()
//    }).ToList();

//    // ✅ Get Food Items
//    var foodItemsList = await _unitOfWork.Product.GetAllAsync();

//    // ✅ Create ViewModel
//    MovieDetailVM viewModel = new MovieDetailVM
//    {
//      Movie = movie,
//        AvailableDates = availableDates,
//        SelectedDate = targetDate,
//        AvailableCities = availableCities,
//        SelectedCity = targetCity,
//        Showtimes = showtimeList,
//        FoodItems = (List<Product>)foodItemsList,
//        Selected = false,
//        TotalPrice = 0
//    };

//    // ✅ Return View with ViewModel
//    return View(viewModel);
//}
