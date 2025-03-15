using Cinema.DataAccess.Data;
using Cinema.Models; // Import model
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class DetailController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách rạp
            var theaters = _context.Cinemas.Where(t => t.Status == CinemaStatus.Open).ToList();

            // Lấy danh sách thành phố duy nhất
            var cities = _context.Cinemas
                                .Where(t => !string.IsNullOrEmpty(t.CinemaCity))
                                .Select(t => t.CinemaCity)
                                .Distinct()
                                .ToList();

            // Truyền dữ liệu qua ViewBag
            ViewBag.Theaters = theaters;
            ViewBag.CinemaCities = cities;

            return View();
        }
    }
}
