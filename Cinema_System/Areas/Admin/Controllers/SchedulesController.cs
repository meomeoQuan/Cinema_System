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
        [HttpPost]
        public async Task<IActionResult> AddRoom(string roomNumber, int cinemaId)
        {
            if (string.IsNullOrEmpty(roomNumber) || cinemaId <= 0)
            {
                return Json(new { success = false, message = "Room number and theater are required." });
            }

            try
            {
                // Tạo đối tượng Room mới
                var newRoom = new Room
                {
                    RoomNumber = roomNumber,
                    CinemaID = cinemaId
                    // Thêm các thuộc tính khác nếu cần (ví dụ: Capacity, Status, v.v.)
                };

                // Thêm vào database qua UnitOfWork
                _unitOfWork.Room.Add(newRoom);
                await _unitOfWork.SaveAsync();

                return Json(new { success = true, message = "Room added successfully", room = new { RoomID = newRoom.RoomID, RoomNumber = newRoom.RoomNumber } });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error adding room: " + ex.Message });
            }
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

        //[HttpGet]
        //public async Task<IActionResult> GetRoomsByCinema(int cinemaId)
        //{
        //    //var rooms = await _unitOfWork.Room.GetRoomsByCinemaIdAsync(cinemaId); old
        //    var rooms = await _unitOfWork.Room.GetAllAsync(r => r.Theater.CinemaID == cinemaId); // quan fix ,xem lai 
        //    // anh muon lay cai gi lay Room dua tren CinemaID hay 
        //    // --- lay Room dua tren roomid include Theater.CinemaID
        //    return Json(rooms);
        //}
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
    }
}
