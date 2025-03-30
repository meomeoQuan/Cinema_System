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
            return Ok(showtimeSeats);
        }

        [HttpPut("{showTimeSeatId}/{status}")]
        public async Task<IActionResult> PutSTSeatStatus(int showTimeSeatId, int status)
        {

            var seat = await _context.showTimeSeats.FindAsync(showTimeSeatId);

            if (seat == null)
            {
                return NotFound("Seat not found.");
            }
            if (!Enum.IsDefined(typeof(ShowtimeSeatStatus), status))
            {
                return BadRequest("Invalid seat status");
            }


            // Cập nhật status
            seat.Status = (ShowtimeSeatStatus)status;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
