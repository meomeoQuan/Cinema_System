// File: /Areas/Admin/Controllers/SchedulesController.cs

using System;
using System.Linq;
using System.Threading.Tasks;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DemoSchedules : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DemoSchedules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Demo()
        {
            var movies = await _unitOfWork.Movie.GetAllAsync();
            var cinemas = await _unitOfWork.Cinema.GetAllAsync();

            ViewBag.Movies = movies.Select(m => new { Id = m.MovieID, m.Title }).ToList();
            ViewBag.Cinemas = cinemas.Select(c => new { Id = c.CinemaID, c.Name }).ToList();

            return View();
        }

        #region API Calls for FullCalendar & AJAX

        // Trong file DemoSchedulesController.cs

        [HttpGet]
        public async Task<IActionResult> GetSchedulesForCalendar(DateTime start, DateTime end)
        {
            var schedules = await _unitOfWork.showTime.GetAllAsync(
                s => s.ShowDate >= DateOnly.FromDateTime(start) && s.ShowDate <= DateOnly.FromDateTime(end),
                includeProperties: "Movie,Room.Theater" // Đảm bảo đã include đầy đủ
            );

            // *** DÒNG SỬA LỖI QUAN TRỌNG ***
            // Thêm .Where() để lọc ra những suất chiếu bị lỗi (không có thông tin phim)
            // Điều này ngăn chặn lỗi 500 Internal Server Error
            var events = schedules
                .Where(s => s.Movie != null && s.Room?.Theater != null)
                .Select(s => new
                {
                    id = s.ShowTimeID,
                    title = s.Movie.Title,
                    start = s.ShowDate.ToDateTime(TimeOnly.FromTimeSpan(s.ShowTimes)),
                    end = s.ShowDate.ToDateTime(TimeOnly.FromTimeSpan(s.ShowTimes)).AddMinutes(s.Movie.Duration),
                    extendedProps = new
                    {
                        roomId = s.RoomID,
                        cinemaName = s.Room.Theater.Name,
                        roomNumber = s.Room.RoomNumber
                    },
                    backgroundColor = GetColorForMovie(s.MovieID)
                }).ToList();

            return Json(events);
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
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = string.Join("\n", errors) });
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
                if (roomEntity != null && roomEntity.Seats.Any())
                {
                    await _unitOfWork.ShowTimeSeat.AddRangeAsync(AutoGenerateTickets(roomEntity, model));
                    await _unitOfWork.SaveAsync();
                }

                return Json(new { success = true, message = "Showtime created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateScheduleFromCalendar(int id, DateTime start)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Invalid event ID." });
            }

            var showtimeFromDb = await _unitOfWork.showTime.GetAsync(s => s.ShowTimeID == id);
            if (showtimeFromDb == null)
            {
                return Json(new { success = false, message = "Showtime not found." });
            }

            var updatedShowtime = new ShowTime
            {
                ShowTimeID = showtimeFromDb.ShowTimeID,
                MovieID = showtimeFromDb.MovieID,
                RoomID = showtimeFromDb.RoomID,
                ShowDate = DateOnly.FromDateTime(start),
                ShowTimes = start.TimeOfDay // .TimeOfDay trả về TimeSpan, đã chính xác
            };

            var validationError = await ValidateShowTime(updatedShowtime, updatedShowtime.ShowTimeID);
            if (validationError != null)
            {
                return Json(new { success = false, message = validationError });
            }

            showtimeFromDb.ShowDate = DateOnly.FromDateTime(start);
            showtimeFromDb.ShowTimes = start.TimeOfDay;

            try
            {
                _unitOfWork.showTime.Update(showtimeFromDb);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "Schedule updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating." });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<string?> ValidateShowTime(ShowTime model, int? ignoreShowtimeId = null)
        {
            if (model.MovieID == 0) return "Movie is required.";
            if (model.RoomID == 0) return "Room is required.";
            if (model.ShowDate < DateOnly.FromDateTime(DateTime.Now)) return "Show date cannot be in the past.";
            if (model.ShowTimes == default) return "Invalid show time.";

            var movie = await _unitOfWork.Movie.GetAsync(m => m.MovieID == model.MovieID);
            if (movie == null || movie.Duration <= 0)
            {
                return "Movie not found or has invalid duration.";
            }

            // *** ĐÂY LÀ DÒNG ĐÃ SỬA ***
            // Chuyển đổi TimeSpan sang TimeOnly trước khi dùng
            var newShowtimeStart = model.ShowDate.ToDateTime(TimeOnly.FromTimeSpan(model.ShowTimes));
            var newShowtimeEnd = newShowtimeStart.AddMinutes(movie.Duration);

            var existingShowtimes = await _unitOfWork.showTime.GetAllAsync(
                s => s.RoomID == model.RoomID &&
                     s.ShowDate == model.ShowDate &&
                     s.ShowTimeID != ignoreShowtimeId,
                includeProperties: "Movie"
            );

            var bufferTime = TimeSpan.FromHours(1);

            foreach (var existingShow in existingShowtimes)
            {
                if (existingShow.Movie == null) continue;

                var existingShowStart = existingShow.ShowDate.ToDateTime(TimeOnly.FromTimeSpan(existingShow.ShowTimes));
                var existingShowEnd = existingShowStart.AddMinutes(existingShow.Movie.Duration);

                bool isConflict = newShowtimeStart < existingShowEnd.Add(bufferTime) &&
                                  newShowtimeEnd.Add(bufferTime) > existingShowStart;

                if (isConflict)
                {
                    return $"Schedule conflict with '{existingShow.Movie.Title}' at {existingShow.ShowTimes:hh\\:mm}. There must be at least a 1-hour gap between showtimes.";
                }
            }

            return null;
        }

        private List<ShowtimeSeat> AutoGenerateTickets(Room room, ShowTime showTime)
        {
            var seats = new List<ShowtimeSeat>();
            foreach (var seat in room.Seats)
            {
                seats.Add(new ShowtimeSeat
                {
                    ShowtimeID = showTime.ShowTimeID,
                    SeatID = seat.SeatID,
                    Status = ShowtimeSeatStatus.Available
                });
            }
            return seats;
        }

        private string GetColorForMovie(int movieId)
        {
            var colors = new string[] { "#3788d8", "#e3bc08", "#e1534a", "#1aada8", "#8b69c7", "#343a40" };
            return colors[movieId % colors.Length];
        }

        #endregion
    }
}