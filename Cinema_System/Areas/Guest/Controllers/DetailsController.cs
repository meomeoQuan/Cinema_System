using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models; // Import model
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class DetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public DetailsController(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int MovieID)
        {
            MovieDetailVM detailVM = new MovieDetailVM()
            {

                Movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID)

            };
            // Lấy danh sách rạp
            var theaters = _context.Theaters.Where(t => t.Status == CinemaStatus.Open).ToList();

            // Lấy danh sách thành phố duy nhất
            var cities = _context.Theaters
                                .Where(t => !string.IsNullOrEmpty(t.CinemaCity))
                                .Select(t => t.CinemaCity)
                                .Distinct()
                                .ToList();

            // Truyền dữ liệu qua ViewBag
            ViewBag.CinemaCities = cities;

            return View(detailVM);
        }
    }
}
