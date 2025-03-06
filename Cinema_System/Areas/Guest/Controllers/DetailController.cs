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
    

    
        public async Task<IActionResult> Details(int MovieID)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Fetch showtimes including Cinema details
            var showTimes = await _unitOfWork.showTime.GetAllAsync(
                u => u.MovieID == MovieID, includeProperties: "Cinema"
            );

            // Create the ViewModel with a structured hierarchy
            MovieDetailVM detailVM = new MovieDetailVM()
            {
                OrderDetails = (List<OrderDetail>)await _unitOfWork.OrderDetail.GetAllAsync(u => u.Order.UserID == userId, includeProperties: "Product"),
                Movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID),
                Cinemas = showTimes
                    .GroupBy(u => new { u.CinemaID, u.Cinema.Name }) // Group by Cinema
                    .Select(cinemaGroup => new CinemaScheduleVM
                    {
                        CinemaID = cinemaGroup.Key.CinemaID,
                        CinemaName = cinemaGroup.Key.Name,
                        AvailableDates = cinemaGroup
                            .Select(show => show.ShowDates) // Extract ShowDate from ShowTime
                            .Distinct()
                            .OrderBy(date => date) // Ensure chronological order
                            .Select(date => new ShowTimeInfoVM
                            {
                                ShowDate = date, // Assign ShowDate
                                ShowTimes = cinemaGroup
                                    .Where(show => show.ShowDates == date) // Filter by ShowDate
                                    .Select(show => show.ShowTimes) // Get show times
                                    .Distinct()
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList(),


            };
            // Debugging: Log fetched showtimes
            foreach (var cart in detailVM.OrderDetails)
            {
              cart.Price += cart.Product.Price;
            }

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
