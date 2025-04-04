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
        private readonly IUnitOfWork _unitOfWork;

        public DetailsController( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int MovieID)
        {
            MovieDetailVM detailVM = new MovieDetailVM()
            {
                Movie = await _unitOfWork.Movie.GetAsync(u => u.MovieID == MovieID)
            };

            return View(detailVM);
        }

    }
}


