using Cinema.DataAccess.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Route("api/showtime")]
    public class ShowTimeController : Controller
    {
        ApplicationDbContext _context;

        public ShowTimeController (ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{cinemaId}/{movieId}")]
        public async Task<IActionResult> GetListShowTime(int cinemaId, int movieId)
        {
            var roomIds = await _context.Rooms
                .Where(r => r.CinemaID == cinemaId)
                .Select(r => r.RoomID)
                .ToListAsync();

            if (!roomIds.Any())
            {
                return NotFound(new { message = "Không tìm thấy phòng nào trong rạp này." });
            }

            var showtimes = await _context.showTimes
                .Where(s => roomIds.Contains(s.RoomID) && s.MovieID == movieId)
                .ToListAsync();

            if (!showtimes.Any())
            {
                return NotFound(new { message = "Không có suất chiếu nào cho phim này trong rạp này." });
            }

            return Ok(showtimes);
        }

    }
}
