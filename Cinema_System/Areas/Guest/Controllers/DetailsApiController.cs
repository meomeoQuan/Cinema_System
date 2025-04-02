using Cinema.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Guest.Controllers
{
    [ApiController]
    [Route("api/details")]
    public class DetailsApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DetailsApiController(ApplicationDbContext context)
        {
            _context = context;
        }   

        [HttpGet("cities")]
        public async Task<IActionResult> getListCity()
        {
            var cities = _context.Theaters
                                .Where(t => !string.IsNullOrEmpty(t.CinemaCity))
                                .Select(t => t.CinemaCity)
                                .Distinct()
                                .ToList();
            return Json(new { data = cities });
        }
    }
}
