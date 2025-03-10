using Cinema.Models.ViewModels;
using Cinema.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Cinema.DataAccess.Repository.IRepository;
using QRCoder;
using System.Drawing.Imaging;
using System.Drawing;

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


        public async Task<IActionResult> Details(int MovieID, string? targetDate = "01/03/2025", string? targetCity = "Danang", string? targetTime = "18:30")
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var showTimes = await _unitOfWork.showTime.GetAllAsync(
                u => u.MovieID == MovieID, includeProperties: "Cinema"
            );

            var showtimeSeats = await _unitOfWork.ShowTimeSeat.GetAllAsync(
                includeProperties: "Seat"
            ); // Fetch all seats related to showtimes

            MovieDetailVM detailVM = new MovieDetailVM()
            {
                Movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID),
                ShowDates = showTimes
                    .Where(show =>
                        (string.IsNullOrEmpty(targetDate) || show.ShowDates == targetDate) &&  // ✅ Date filter
                        (string.IsNullOrEmpty(targetCity) || string.Equals(show?.Cinema?.CinemaCity?.ToLower().Trim(), targetCity.ToLower().Trim(), StringComparison.OrdinalIgnoreCase)) && // ✅ City filter
                        (string.IsNullOrEmpty(targetTime) || show.ShowTimes == targetTime) // ✅ Time filter
                    )
                    .GroupBy(show => show.ShowDates) // Group by Date
                    .OrderBy(g => g.Key) // Sort Dates
                    .Select(dateGroup => new ShowDateVM
                    {
                        ShowDate = dateGroup.Key,
                        Cities = dateGroup.GroupBy(show => show.Cinema.CinemaCity) // Group by City
                            .Select(cityGroup => new CityVM
                            {
                                CityName = cityGroup.Key,
                                Cinemas = cityGroup.GroupBy(show => new { show.CinemaID, show.Cinema.Name })
                                    .Select(cinemaGroup => new TimeScheduleVM
                                    {
                                        CinemaID = cinemaGroup.Key.CinemaID,
                                        CinemaName = cinemaGroup.Key.Name,
                                        ShowTimes = cinemaGroup
                                            .Where(show => string.IsNullOrEmpty(targetTime) || show.ShowTimes == targetTime) // ✅ Apply Time Filter here
                                            .Select(show => show.ShowTimes)
                                            .Distinct()
                                            .OrderBy(time => time) // Sort ShowTimes
                                            .ToList(),
                                        Seats = cinemaGroup
                                            .SelectMany(show => showtimeSeats
                                                .Where(seat => seat.ShowtimeID == show.ShowTimeID)
                                                .GroupBy(seat => seat.Seat.Row)
                                                .OrderBy(group => group.Key) // Order rows alphabetically
                                                .Select(rowGroup => new CinemaSeatVM
                                                {
                                                    Row = rowGroup.Key,
                                                    listSeatGrid = rowGroup
                                                        .OrderBy(seat => seat.Seat.ColumnNumber)
                                                        .Select(seat => seat.Seat.SeatName + " (" + seat.Status + ")")
                                                        .ToList()
                                                })
                                            )
                                            .ToList()
                                    })
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return View(detailVM);
        }




        [HttpPost]
        public IActionResult Create(int MovieID, int CinemaID, string ShowDate, string ShowTime)
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
