using Cinema.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_System.Areas.Guest.Controllers
{
    [ApiController]
    [Route("api/showtime-seat")]
    public class ShowtimeSeatApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShowtimeSeatApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{showtimeID}")]
        public async Task<IActionResult> GetShowtimeSeats(int showtimeID)
        {
            var showtimeSeats = await _context.showTimeSeats
                .Where(s => s.ShowtimeID == showtimeID)
                .ToListAsync();
            if (!showtimeSeats.Any())
            {
                return NotFound(new { message = "Không tìm thấy ghế nào trong suất chiếu này." });
            }
            return Ok(showtimeSeats);
        }
    }
}
