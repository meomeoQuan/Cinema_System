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
            //var schedules = await _unitOfWork.showTime.GetAllAsync("Movie,Cinema"); old 
            var schedules = await _unitOfWork.showTime.GetAllAsync(includeProperties: "Movie,Room.Cinema"); // quan fix new
            return View(schedules);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            // Sử dụng includeProperties để load dữ liệu từ các bảng liên quan
            var schedules = await _unitOfWork.showTime.GetAllAsync(includeProperties: "Movie,Room.Cinema");// new

            //var schedules = await _unitOfWork.showTime.GetAllAsync(includeProperties: "Movie,Cinema"); old


            // Kiểm tra xem dữ liệu đã được load đúng cách chưa
            var schedulesList = schedules.Select(s => new
            {
                MovieName = s.Movie?.Title ?? "Unknown", // Kiểm tra null
                //CinemaName = s.Cinema?.Name ?? "Unknown", // Kiểm tra null  old 
                CinemaName = s?.Room?.Cinema?.Name ?? "Unknown",
                s.RoomID,

                //s.ShowDates, // old
                //s.ShowTimes,// old 

                s.ShowDate, // new -> contains both date and time ex : 2025-03-10 07:30:00.0000000
                s.AvailableTicketQuantity
            }).ToList();

            return Json(new { data = schedulesList });
        }
    }// quan fix
}
