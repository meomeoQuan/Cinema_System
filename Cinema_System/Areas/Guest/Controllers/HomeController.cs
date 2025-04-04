using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Security.Claims;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Cinema_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        //vua in vua tim kiem
        //[HttpGet]
        //public async Task<IActionResult> Index(string term)
        //{
        //    IEnumerable<Movie> movies;

        //    if (!string.IsNullOrEmpty(term))
        //    {
        //        movies = await _unitOfWork.Movie.SearchAsync(term);
        //        ViewBag.SearchTerm = term; // giu lai tu khoa tim kiem
        //    }
        //    else
        //    {
        //        movies = await _unitOfWork.Movie.GetAllAsync();
        //    }

        //    return View(movies);
        //}
        [HttpGet]
        public async Task<IActionResult> Index(string term)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            string? userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                ApplicationUser? user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
                if (user != null)
                {
                    ViewData["UserFullname"] = user.FullName;
                    ViewData["UserImage"] = user.UserImage;
                }
            }
            //-------------------------------------------------------------------------------
            // Search functionality
            if (!string.IsNullOrEmpty(term))
            {
                var movies = await _unitOfWork.Movie.SearchAsync(term);
                ViewBag.SearchTerm = term; // Keep the search term in the ViewBag
                return View(movies);
            }

            return View(new List<Movie>()); // Return an empty list if no search term is provided
        }


        //[HttpGet]
        //public async Task<IActionResult> Index(string term)
        //{
        //    if (!string.IsNullOrEmpty(term))
        //    {

        //        var movies = await _unitOfWork.Movie.SearchAsync(term, null); // them null de khong filter
        //        ViewBag.SearchTerm = term;
        //        return View(movies);
        //    }
        //    return View(new List<Movie>());
        //}

        public IActionResult Chat()
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


        public async Task<IActionResult> Product()
        {
            return View();
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
