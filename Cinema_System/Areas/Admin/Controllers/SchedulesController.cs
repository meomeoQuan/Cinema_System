using System;
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

        // Index() and GetRoomsByCinema() methods remain the same...
        public async Task<IActionResult> Index()
        {
            var schedules = await _unitOfWork.showTime.GetAllAsync("Movie,Room.Theater");
            var movies = await _unitOfWork.Movie.GetAllAsync();
            var cinemas = await _unitOfWork.Cinema.GetAllAsync();

            ViewBag.Movies = movies.Select(m => new { Id = m.MovieID, m.Title }).ToList();
            ViewBag.Cinemas = cinemas.Select(c => new { Id = c.CinemaID, c.Name }).ToList();
            return View(schedules);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomsByCinema(int cinemaId)
        {
            if (cinemaId == 0)
            {
                return Json(new { success = false, message = "Invalid Cinema ID." });
            }
            var rooms = await _unitOfWork.Room.GetAllAsync(r => r.CinemaID == cinemaId);
            var roomList = rooms.Select(r => new { r.RoomID, r.RoomNumber }).ToList();
            return Json(new { success = true, rooms = roomList });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ShowTime model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            var validationError = await ValidateShowTime(model);
            if (validationError != null)
            {
                return Json(new { success = false, message = validationError });
            }

            try
            {
                _unitOfWork.showTime.Add(model);
                await _unitOfWork.SaveAsync();

                var roomEntity = await _unitOfWork.Room.GetAsync(r => r.RoomID == model.RoomID, includeProperties: "Seats");

                if (roomEntity == null)
                {
                    return Json(new { success = false, message = "Invalid room." });
                }
                await _unitOfWork.ShowTimeSeat.AddRangeAsync(AutoGenerateTickets(roomEntity, model));

                await _unitOfWork.SaveAsync();


                return Json(new { success = true, message = "Showtime created successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateShowtime(ShowTime updatedShowtime)
        {
            if (updatedShowtime == null || updatedShowtime.ShowTimeID == 0)
            {
                return Json(new { success = false, message = "Invalid data provided." });
            }

            var showtimeFromDb = await _unitOfWork.showTime.GetAsync(s => s.ShowTimeID == updatedShowtime.ShowTimeID);
            if (showtimeFromDb == null)
            {
                return Json(new { success = false, message = "Showtime not found." });
            }

            // Không có gì thay đổi
            if (showtimeFromDb.ShowDate == updatedShowtime.ShowDate &&
                showtimeFromDb.ShowTimes == updatedShowtime.ShowTimes &&
                showtimeFromDb.MovieID == updatedShowtime.MovieID &&
                showtimeFromDb.RoomID == updatedShowtime.RoomID)
                return Json(new { message = "No changes were detected." });

            var validationError = await ValidateShowTime(updatedShowtime, updatedShowtime.ShowTimeID);
            if (validationError != null)
            {
                return Json(new { success = false, message = validationError });
            }

            // Update properties
            showtimeFromDb.MovieID = updatedShowtime.MovieID;
            showtimeFromDb.RoomID = updatedShowtime.RoomID;
            showtimeFromDb.ShowDate = updatedShowtime.ShowDate;
            showtimeFromDb.ShowTimes = updatedShowtime.ShowTimes;

            try
            {
                _unitOfWork.showTime.Update(showtimeFromDb);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "Showtime updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return Json(new { success = false, message = "An error occurred while updating." });
            }
        }

        // ========== THIS IS THE CORRECTED METHOD ==========
        private async Task<string?> ValidateShowTime(ShowTime model, int? ignoreShowtimeId = null)
        {
            if (model.MovieID == 0) return "Movie is required.";
            if (model.RoomID == 0) return "Room is required.";
            if (model.ShowDate < DateOnly.FromDateTime(DateTime.Now)) return "Show date cannot be in the past.";
            if (model.ShowTimes == default) return "Invalid show time.";

            // Use AnyAsync for an efficient existence check that doesn't cause a concurrency conflict.
            bool isConflict = await _unitOfWork.showTime.AnyAsync(s =>
                s.RoomID == model.RoomID &&
                s.ShowDate == model.ShowDate &&
                s.ShowTimes == model.ShowTimes &&
                s.ShowTimeID != ignoreShowtimeId); // This check correctly ignores the current entity being edited

            if (isConflict)
            {
                return "A showtime already exists for this room at the selected date and time.";
            }

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