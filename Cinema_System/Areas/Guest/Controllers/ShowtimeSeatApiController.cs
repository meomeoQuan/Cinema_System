using Cinema.DataAccess.Data;
using Cinema.Models;
using Cinema_System.Areas.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

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

        [HttpPost]
        public async Task<IActionResult> GetShowtimeSeatByShowtimeIdtAndSeatId( [FromBody] ShowTimeSearchRequest request)
        {
            var showtimeSeats = new ArrayList();
            foreach (int seatId in request.seatIds)
            {
                var stSeat = await _context.showTimeSeats
                .FirstOrDefaultAsync(s => s.ShowtimeID == request.showTimeId && s.SeatID == seatId);
                showtimeSeats.Add(stSeat);
            }
            
            if (!(showtimeSeats.Count > 0))
            {
                return NotFound(new { message = "Không tìm thấy ghế nào trong suất chiếu này." });
            }
            return Ok(showtimeSeats);
        }
    }
}
