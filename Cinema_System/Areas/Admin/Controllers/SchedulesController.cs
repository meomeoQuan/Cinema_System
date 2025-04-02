﻿using System.Linq;
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

        //[HttpGet("GetAll")]
        //public async Task<IActionResult> GetAll()
        //{
        //    // Sử dụng includeProperties để load dữ liệu từ các bảng liên quan
        //    var schedules = await _unitOfWork.showTime.GetAllAsync(includeProperties: "Movie,Cinema");

        //    // Kiểm tra xem dữ liệu đã được load đúng cách chưa
        //    var schedulesList = schedules.Select(s => new
        //    {
        //        MovieName = s.Movie?.Title ?? "Unknown", // Kiểm tra null
        //        CinemaName = s.Cinema?.Name ?? "Unknown", // Kiểm tra null
        //        s.RoomID,
        //        s.ShowDates,
        //        s.ShowTimes,
        //        s.AvailableTicketQuantity
        //    }).ToList();

        //    return Json(new { data = schedulesList });
        //}

        [HttpGet]
        public async Task<IActionResult> GetRoomsByCinema(int cinemaId)
        {
            //var rooms = await _unitOfWork.Room.GetRoomsByCinemaIdAsync(cinemaId); old
            //var rooms = await _unitOfWork.Room.GetAllAsync(r => r.Theater.CinemaID == cinemaId); // quan fix ,xem lai 

            // i wanna get room base on cinemaId
            var rooms = await _unitOfWork.Room.GetAllAsync(r => r.CinemaID == cinemaId);
            foreach (var item in rooms)
            {
                Console.WriteLine(item.RoomNumber);
            }
            // anh muon lay cai gi lay Room dua tren CinemaID hay 
            // --- lay Room dua tren roomid include Theater.CinemaID
            return Json(new { success = true, rooms = rooms });
        }
        
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShowTime model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid showtime data." });
            }

            // Check if room exists
            var room = await _unitOfWork.Room.GetAllAsync(r => r.RoomID == model.RoomID, includeProperties: "Seats");
            
            var roomEntity = room.FirstOrDefault();
            if (roomEntity == null)
            {
                return Json(new { success = false, message = "Invalid room." });
            }

            _unitOfWork.showTime.Add(model);
            await _unitOfWork.SaveAsync();

            await _unitOfWork.ShowTimeSeat.AddRangeAsync(AutoGenerateTickets(roomEntity, model));

            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Showtime and tickets created successfully!" });
        }


        private List<ShowtimeSeat>  AutoGenerateTickets(Room room, ShowTime showTime)
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
