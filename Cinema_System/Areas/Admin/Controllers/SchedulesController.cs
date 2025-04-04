using System.Linq;
using System.Threading.Tasks;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SchedulesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SchedulesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var schedules = await _unitOfWork.showTime.GetAllAsync("Movie,Room.Theater");
            var movies = await _unitOfWork.Movie.GetAllAsync();
            var cinemas = await _unitOfWork.Cinema.GetAllAsync();
            //var
            //var Showtime
            ViewBag.Movies = movies.Select(m => new { Id = m.MovieID, Title = m.Title }).ToList();
            ViewBag.Cinemas = cinemas.Select(c => new { Id = c.CinemaID, Name = c.Name }).ToList();
            return View(schedules);
        }
        [HttpGet]
        public async Task<IActionResult> GetRoomsByCinema(int cinemaId)
        {
            var rooms = await _unitOfWork.Room.GetAllAsync(r => r.Theater.CinemaID == cinemaId);

            var roomList = rooms.Select(r => new
            {
                RoomID = r.RoomID,
                RoomNumber = r.RoomNumber
            }).ToList();

            return Json(new { success = true, rooms = roomList });
        }
        [HttpPost]
        public async Task<IActionResult> Create(ShowTime model)
        {
            var validationError = await ValidateShowTime(model);
            if (validationError != null)
            {
                return Json(new { success = false, message = validationError });
            }
            try
            {
                Console.WriteLine($"Model: {model.ShowDate}, {model.ShowTimes}, {model.RoomID}");

                _unitOfWork.showTime.Add(model);
                await _unitOfWork.SaveAsync();

                return Json(new { success = true, message = "Showtime created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        private async Task<string?> ValidateShowTime(ShowTime model)
        {
            if (model.RoomID == 0)
                return "Room is required.";
            if (model.ShowDate == default)
                return "Invalid show date.";
            if (model.ShowDate <= DateOnly.FromDateTime(DateTime.Now))
                return "Show date must be in the future.";
            if (model.ShowTimes == default)
                return "Invalid show time.";
            var room = await _unitOfWork.Room.GetAllAsync(r => r.RoomID == model.RoomID);
            if (room == null)
                return "Invalid room selection.";
            var existingShowtime = await _unitOfWork.showTime.GetAllAsync(
                s => s.RoomID == model.RoomID &&
                     s.ShowDate == model.ShowDate &&
                     s.ShowTimes == model.ShowTimes);

            if (existingShowtime != null)
                return "A showtime already exists for this room at this time.";

            return null;
        }

        private List<ShowtimeSeat> AutoGenerateTickets(Room room, ShowTime showTime)
        {
            var seats = new List<ShowtimeSeat>();
            foreach (var seat in room.Seats)
            {
                var showtimeSeat = new ShowtimeSeat
                {
                    ShowtimeID = showTime.ShowTimeID,
                    SeatID = seat.SeatID,
                    Status = ShowtimeSeatStatus.Available
                };
                seats.Add(showtimeSeat);
            }
            return seats;
        }

    }
}