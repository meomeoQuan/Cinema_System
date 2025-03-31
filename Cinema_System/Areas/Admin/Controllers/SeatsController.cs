using System.Collections.Generic;
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

        public async Task<IActionResult> GetAll(int roomId)
        {
            if (roomId <= 0)
            {
                return Json(new { success = false, message = "Invalid Room ID." });
            }

            var seats = await _unitOfWork.Seat.GetSeatsByRoomIdAsync(roomId);
            return Json(new { success = true, data = seats });
        }

        [HttpGet]
        public async Task<IActionResult> GetSeatById(int seatId)
        {
            if (seatId <= 0)
            {
                return BadRequest("Invalid Seat ID.");
            }

            var seat = await _unitOfWork.Seat.GetByIdAsync(seatId);
            if (seat == null)
            {
                return NotFound();
            }

            return Json(seat);
        }
    }
}
