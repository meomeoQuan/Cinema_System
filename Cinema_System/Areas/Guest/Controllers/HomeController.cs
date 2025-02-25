using System.Diagnostics;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Cinema_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> GetMovies(int Showingpage = 1, int Upcommingpage = 1, int CouponPage = 1)
        {
            // Ensure page numbers are always 1 or greater
            Showingpage = Math.Max(1, Showingpage);
            Upcommingpage = Math.Max(1, Upcommingpage);
            CouponPage = Math.Max(1, CouponPage);
            // hien thi moi trang 4 cai 
            int pageSize = 4;
            var showingMovies = await _unitOfWork.Movie.GetAllPagedAsync(Showingpage, pageSize, u => !u.IsUpcomingMovie);
            var upcommingMovies = await _unitOfWork.Movie.GetAllPagedAsync(Upcommingpage, pageSize, u => u.IsUpcomingMovie);
            var couponMovies = await _unitOfWork.Coupon.GetAllPagedAsync(CouponPage, pageSize);

            var movieVM = new MovieVM()
            {
                ShowingMovies = showingMovies,
                UpcommingMovies = upcommingMovies,
                CouponMovies = couponMovies,

                ShowingMoviesCount = await _unitOfWork.Movie.CountAsync(u => !u.IsUpcomingMovie),
                UpcommingMoviesCount = await _unitOfWork.Movie.CountAsync(u => u.IsUpcomingMovie),
                CouponCount = await _unitOfWork.Coupon.CountAsync(),
                PageSize = pageSize
            };

            //ViewBag.CurrentPage = Showingpage; // Pass current page to view
            //ViewBag.CurrentPage = Upcommingpage; // Pass current page to view
            //ViewBag.CurrentPage = CouponPage; // Pass current page to view

            return Ok(new { data = movieVM, message = "Success" });
        }

        #endregion

        public async Task<IActionResult> Details(int MovieID)
        {
            var showTimes = await _unitOfWork.showTime.GetAllAsync(u => u.MovieID == MovieID);

            MovieDetailVM detailVM = new MovieDetailVM()
            {
                Movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID),

                // Populate Cinemas
                CinemaListItem = showTimes.Select(u => new SelectListItem
                {
                    Value = u.CinemaID.ToString(),
                    Text = u.Cinema.Name
                }).Distinct().ToList(),

                // Populate Dates
                DateListItem = showTimes.Select(u => new SelectListItem
                {
                    Value = u.ShowDate.ToString("yyyy-MM-dd"),
                    Text = u.ShowDate.ToString("dd/MM/yyyy")
                }).Distinct().ToList(),

                // Populate ShowTimes
                ShowTimeListItem = showTimes.Select(u => new SelectListItem
                {
                    Value = u.ShowTimeID.ToString(),
                    Text = u.ShowTimes.ToString("hh:mm tt")
                }).Distinct().ToList()
            };

            return View(detailVM);
        }


        public async Task<IActionResult> Cart()
        {
            return View();
        }

        public async Task<IActionResult> Showing()
        {
            IEnumerable<Movie> movies = await _unitOfWork.Movie.GetAllAsync(u => !u.IsUpcomingMovie);


            return View(movies);
        }


        public async Task<IActionResult> Upcomming()
        {
            IEnumerable<Movie> movies = await _unitOfWork.Movie.GetAllAsync(u => u.IsUpcomingMovie);


            return View(movies);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
