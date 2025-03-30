using System.Linq;
using System.Threading.Tasks;
using Cinema.DataAccess.Repository.IRepository;
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
            var rooms = await _unitOfWork.Room.GetAllAsync(r => r.Theater.CinemaID == cinemaId); // quan fix ,xem lai 
            // anh muon lay cai gi lay Room dua tren CinemaID hay 
            // --- lay Room dua tren roomid include Theater.CinemaID
            return Json(rooms);
        }
    }
}
