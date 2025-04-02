using System.Threading.Tasks;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class SeatsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SeatsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int? roomId)
        {
            if (!roomId.HasValue || roomId <= 0)
            {
                return NotFound("Room ID is required.");
            }

            var seats = await _unitOfWork.Seat.GetSeatsByRoomIdAsync(roomId.Value);
            ViewData["RoomId"] = roomId.Value;
            return View(seats);
        }

        [HttpGet]
        public async Task<IActionResult> GetSeatById(int seatId)
        {
            if (seatId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid Seat ID." });
            }

            var seat = await _unitOfWork.Seat.GetByIdAsync(seatId);
            if (seat == null)
            {
                return NotFound(new { success = false, message = "Seat not found." });
            }

            return Json(new { success = true, data = seat });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int seatId, string action)
        {
            if (seatId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid Seat ID." });
            }

            var seat = await _unitOfWork.Seat.GetByIdAsync(seatId);
            if (seat == null)
            {
                return NotFound(new { success = false, message = "Seat not found." });
            }

            if (action == "lock")
            {
                seat.Status = SeatStatus.Maintenance;
            }
            else if (action == "unlock")
            {
                seat.Status = SeatStatus.Available;
            }
            else
            {
                return BadRequest(new { success = false, message = "Invalid action." });
            }

            _unitOfWork.Seat.Update(seat);
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Seat status updated successfully." });
        }
    }
}
