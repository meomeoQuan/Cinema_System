using Cinema.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_System.Areas.Guest.Controllers
{
    [ApiController]
    [Route("api/seats")]
    public class SeatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeatController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GetSeats([FromBody] List<int> seatIds)
        {
            var seats = await _context.Seats
                .Where(s => seatIds.Contains(s.SeatID))
                .ToListAsync();
            if (!seats.Any())
            {
                return NotFound(new { message = "Không tìm thấy ghế nào." });
            }
            return Ok(seats);
        }
    }
}
