using Cinema.Models.ViewModels;
using Cinema.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Cinema.DataAccess.Repository.IRepository;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class DetailController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public DetailController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //C1
        //public async Task<IActionResult> Index(int MoviesId)
        //{
        //    return View();
        //}

        //C1
        public async Task<IActionResult> Details(int MovieID, string? targetDate = "01/03/2025", string? targetCity = "Danang", string? targetTime = null)
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

            // ✅ Filter showtimes based on user input
            var filteredShowtimes = showTimes
                .Where(show =>
                    (string.IsNullOrEmpty(targetDate) || show.ShowDates == targetDate) &&
                    (string.IsNullOrEmpty(targetCity) ||
                     string.Equals(show?.Cinema?.CinemaCity?.Trim(), targetCity.Trim(), StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(targetTime) || show?.ShowTimes == targetTime)
                )
                .ToList();


            // Get available cities from the showtimes
            var availableCities = showTimes
                .Select(show => show.Cinema.CinemaCity)
                .Distinct()
                .ToList();

            // If targetDate is null, get all unique show dates
            var availableDates = showTimes
                .Select(show => show.ShowDates)
                .Distinct()
                .ToList();



            if (!filteredShowtimes.Any())
            {
                return NotFound("No matching showtime found.");
            }

            // ✅ Create a list of cinema, room, and showtime combinations
            var showtimeList = filteredShowtimes.Select(show => new
            {
                CinemaName = show.Cinema.Name,
                RoomId = show.Room.RoomID,
                RoomName = show.Room.RoomNumber, // or RoomName if available
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

            // ✅ Get Food Items
            var foodItems = await _unitOfWork.Product.GetAllAsync(); // Assuming you have a Food entity
            var selectedFoodItems = foodItems
                .Take(1) // Dummy selection logic, modify as needed
                .Select(food => new
                {
                    FoodId = food.ProductID,
                    FoodName = food.Name,
                    Quantity = 2, // Default quantity for example
                    Price = food.Price
                })
                .ToList();

            // ✅ Calculate Total Price (Seats + Food)
            double totalPrice = showtimeList.Sum(st => st.Tickets.Sum(t => t.Price)) + selectedFoodItems.Sum(f => f.Price * f.Quantity);

            // ✅ Return structured response
            var result = new
            {
                UserId = userId,
                MovieId = movie.MovieID,
                MovieName = movie.Title,
                AvailableDates = availableDates, // List of available dates
                SelectedDate = targetDate,
                City = targetCity,
                AvailableCities = availableCities, // List of available cities
                Showtimes = showtimeList, // ✅ Multiple cinemas, rooms, showtimes
                FoodItems = selectedFoodItems,
                TotalPrice = totalPrice // Updated total price
            };

            return Json(new { message = "Success", data = result });
        }



        //C2
        //    public async Task<IActionResult> Details(int MovieID, string? targetDate = null , string? targetCity = null, string? targetTime = null)
        //    {
        //        // ✅ Get User ID from Claims
        //        var claimsIdentity = (ClaimsIdentity)User.Identity;
        //        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //        // ✅ Fetch Movie Details
        //        var movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID);
        //        if (movie == null)
        //        {
        //            return NotFound("Movie not found.");
        //        }

        //        // ✅ Fetch Showtimes for the selected movie
        //        var showTimes = await _unitOfWork.showTime.GetAllAsync(
        //            u => u.MovieID == MovieID, includeProperties: "Cinema,Room"
        //        );

        //        // ✅ Fetch Showtime Seats
        //        var showtimeSeats = await _unitOfWork.ShowTimeSeat.GetAllAsync(
        //            includeProperties: "Seat"
        //        );

        //        // ✅ Filter showtimes based on user input
        //        var filteredShowtimes = showTimes
        //            .Where(show =>
        //                (string.IsNullOrEmpty(targetDate) || show.ShowDates == targetDate) &&
        //                (string.IsNullOrEmpty(targetCity) ||
        //                 string.Equals(show?.Cinema?.CinemaCity?.Trim(), targetCity.Trim(), StringComparison.OrdinalIgnoreCase)) &&
        //                (string.IsNullOrEmpty(targetTime) || show?.ShowTimes == targetTime)
        //            )
        //            .ToList();

        //        if (!filteredShowtimes.Any())
        //        {
        //            return NotFound("No matching showtime found.");
        //        }

        //        // ✅ Create a list of cinema, room, and showtime combinations
        //        var showtimeList = filteredShowtimes.Select(show => new
        //        {
        //            CinemaName = show.Cinema.Name,
        //            RoomId = show.Room.RoomID,
        //            RoomName = show.Room.RoomNumber, // or RoomName if available
        //            Showtime = show.ShowTimes,
        //            Tickets = showtimeSeats
        //                .Where(seat => seat.ShowtimeID == show.ShowTimeID)
        //                .Select(seat => new
        //                {
        //                    SeatId = seat.SeatID,
        //                    SeatNumber = seat.Seat.SeatName,
        //                    SeatType = seat.SeatType.ToString(),
        //                    Price = seat.Price
        //                })
        //                .ToList()
        //        }).ToList();

        //        // ✅ Get Food Items
        //        var foodItems = await _unitOfWork.Product.GetAllAsync(); // Assuming you have a Food entity
        //        var selectedFoodItems = foodItems
        //            .Take(1) // Dummy selection logic, modify as needed
        //            .Select(food => new
        //            {
        //                FoodId = food.ProductID,
        //                FoodName = food.Name,
        //                Quantity = 2, // Default quantity for example
        //                Price = food.Price
        //            })
        //            .ToList();

        //        // ✅ Calculate Total Price (Seats + Food)
        //        double totalPrice = showtimeList.Sum(st => st.Tickets.Sum(t => t.Price)) + selectedFoodItems.Sum(f => f.Price * f.Quantity);

        //        // Assume showtimeList is defined earlier, for example:


        //        var model = new MovieDetailVM
        //        {
        //            Movie = new Movie
        //            {
        //                MovieID = movie.MovieID,
        //                Title = movie.Title,
        //                MovieImage = movie.MovieImage,
        //                Genre = movie.Genre,
        //                Duration = movie.Duration,
        //                ReleaseDate = movie.ReleaseDate,
        //                Synopsis = movie.Synopsis,
        //                TrailerLink = movie.TrailerLink
        //            },
        //            // Build the ShowDates list – here we create one entry for the selected date.
        //            ShowDates = new List<ShowDateVM>
        //{
        //    new ShowDateVM
        //    {
        //        ShowDate = targetDate, // the selected date
        //        Cities = new List<CityVM>
        //        {
        //            new CityVM
        //            {
        //                CityName = targetCity,
        //                Cinemas = showtimeList.Select(show => new TimeScheduleVM
        //                {
        //                    CinemaID = show.RoomId, // or show.CinemaID if available
        //                    CinemaName = show.CinemaName,
        //                    ShowTimes = new List<string> { show.Showtime },
        //                    // Build the dictionary: Key is the showtime, Value is the mapped tickets list
        //                    SeatsByShowtime = new Dictionary<string, List<CinemaSeatVM>>
        //                    {
        //                        {
        //                            show.Showtime,
        //                            show.Tickets.Select(ticket => new CinemaSeatVM
        //                            {
        //                                SeatId = ticket.SeatId,
        //                                SeatName = ticket.SeatNumber,
        //                                // For simplicity, using the first character of the seat number as the Row.
        //                                Row = ticket.SeatNumber.Substring(0, 1),
        //                                // Parse the string value into the TicketType enum.
        //                                SeatType = (TicketType)Enum.Parse(typeof(TicketType), ticket.SeatType),
        //                                // Set the Price dynamically from the ticket's price.
        //                                Price = ticket.Price,
        //                                // Set status as available.
        //                                Status = ShowtimeSeatStatus.Available
        //                            }).ToList()
        //                        }
        //                    }
        //                }).ToList()
        //            }
        //        }
        //    }
        //},
        //            // Initialize shopping cart items as empty
        //            SelectedSeats = new List<TicketSelectionVM>(),
        //            SelectedFoodItems = new List<FoodSelectionVM>(),
        //            TotalPrice = 0
        //        };

        //        return View(model);



        //    }
        //  trong phương thức này sẽ theo quy trình list (ngày)) chọn ngày -> hiển thị list city tương ứng (chọn city))
        // -> hiển thị list cinema có trong thành phố , trong từng cinema sẽ có list giờ tương ứng 
        // chọn giờ thì hiển thi ra sơ đồ ghế 


        //  vd :   08/03/2025 -- pick     11/03/2025     15/03/2025

        //   city: Danang -- pick ,HCM, ..
        //  list cinema    :      CGV danang 18:30 <-- pick , 19:30        CGV haichau danang 19:00
        // chọn vé
        // hien thi ghe ngoi 
        // chọn ghế ngồi 
        //chọn food
        // submit 
        // tham khảo trên cinestar detail  film




        // hóa đơn phải chứa tất cả thông tin được định nghĩa trong Orderdetail 



        //[NotMapped]
        //public List<TicketSelectionVM> Tickets { get; set; } = new List<TicketSelectionVM>();

        //// List of selected food items (multiple items possible)
        //[NotMapped]
        //public List<FoodSelectionVM> FoodItems { get; set; } = new List<FoodSelectionVM>();


        // 2 cái trên có nghĩa là người dùng có thể mua đc nhiều vé , nhiều food khác nhau , nên ghi duoiws dạng list 



        // tham khảo trên cinestar detail  film 
        // hiện tại seed data test là movieid = 1 , showtimeid = 1 




        // tui đã thử 2 cách 1 cách là làm json  như trên thì phải có 2 phương thức 

        // 1 là INdex chịu trách nhiệm trỏ  tới trang hiển thị 
        // 2 là details chịu trách nhiệm xử lý logic sau đo pass logic tới detail.js để xử lý giao diện rồi mới chuyển tới Index để hiển thị đầy đủ 
        // vì vậy vào trang homepage.js ở wwwroot -> js -> hômepage 

        //         <a href = "/Guest/Detail/Details?MovieID=${movie.movieID}" class="btn btn-outline-warning">
        //                          ${movie.isUpcomingMovie? "Detail" : "Book Ticket"}
        //</a>                    </a>
        /// <summary>
        /// nêú làm theo cách 1 thì /Guest/Detail/Index?MovieID=${movie.movieID}
        /// vì vậy thì thêm 1 cái public IactionResult (int MovieID) -> nhận giá trị truyền vào 
        /// đây là lúc cấn tui ko bt làm sao để từ INdex pass qua Detail.js để xử lý logic 
        /// 
        /// giống như trong phân trang ở homeController , và homepage.js 
        /// tui nghĩ phần ni sẽ khó cho Thanh 
        /// </summary>
        /// 





        // lý do  tại sao json phải dể INdex làm file chiếu thì vì nếu để Details nó chỉ hiển thị JSon form thôi 
        // ko qua đc Details.html 







        /// <summary>
        /// cách 2 dùng theo MVC thì xóa INdex đi chỉ dùng Detail thôi
        /// lúc này thì  <a href = "/Guest/Detail/Details?MovieID=${movie.movieID}" class="btn btn-outline-warning">
        //                          ${movie.isUpcomingMovie? "Detail" : "Book Ticket"}
        //</a>                    </a>
        /// lúc này Details return về view(model) chứ ko phải là JOSon nữa vì model là enomous model 
        /// nên cần nó chuyển về movieDEtailVM để hiển thị thoong tin và trong Details.cshtml sẽ đinghj nghĩa @model MovieDetalVM -> đây là class để hiển thị chính  
        /// trong ni thì model MOvie để hiển thị movie details , showdate để hiển thì date và city , từ city sẽ lấy ra đc cinema và time 
        /// trong lít ticketSelection chứa ghế ngồi hiện tại là showtime 1 , room 1 là 50 cái 
        /// lit food là thức ăn lựa chọn 
        /// tôtal là tiền tổng 
        /// </summary>




        /// <summary>
        ///  chonj C1 thì dùng json detail, mở lại Index (xử lý vấn đề pass id ) , đổi tên Details.cshtml -> Index.html 
        /// chọn C2 thì xóa Index dùng Details thôi , INdex.cshmtl -> Details.cshtml , xử lý vấn đề về logic return 
        /// C3 xóa hết tự mi làm lại --- mi dễ chọn cách ni lắm 
        /// </summary>

        // mục tiêu là hiển thị hết thông tin usser chọn đc break point ở order post nhận hết thông tin , hiện tại ỏder tui chưa đủ tham số
        // tham khảo thông tin Order sẽ nhận ở Ỏderdetail model .

        [HttpPost]
        public IActionResult  Order(int MovieID, int CinemaID, string ShowDate, string ShowTime)
        {
            // 1. Validate inputs
            // 2. Process booking (e.g., save to database)
            // 3. Redirect to confirmation page

            return RedirectToAction("Confirmation", new
            {
                MovieID = MovieID,
                CinemaID = CinemaID,
                ShowDate = ShowDate,
                ShowTime = ShowTime
            });
        }

        public IActionResult Confirmation(int MovieID, int CinemaID, string ShowDate, string ShowTime)
        {
            var bookingDetails = new
            {
                MovieID = MovieID,
                CinemaID = CinemaID,
                ShowDate = ShowDate,
                ShowTime = ShowTime
            };

            return View(bookingDetails);
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
