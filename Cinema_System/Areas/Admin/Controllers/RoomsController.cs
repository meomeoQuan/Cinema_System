﻿using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)] 
    public class RoomsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoomsController(IUnitOfWork unitOfWork,
                               UserManager<IdentityUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(int cinemaId)
        {
            ViewData["CinemaId"] = cinemaId;
            var list = await _unitOfWork.Room.GetAllAsync(r => r.CinemaID == cinemaId, includeProperties: "Theater");
            
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomsByCinema(int cinemaId)
        {
            if (cinemaId <= 0)
            {
                return Json(new { success = false, message = "Invalid cinema ID" });
            }

            try
            {
                var rooms = await _unitOfWork.Room.GetAllAsync(r => r.CinemaID == cinemaId);

                if (rooms == null || !rooms.Any())
                {
                    return Json(new { success = false, message = "No rooms found", rooms = new List<object>() });
                }

                return Json(new { success = true, rooms = rooms.Select(r => new { r.RoomID, r.RoomNumber }) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving rooms: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room updatedRoom)
        {
            if (!ModelState.IsValid) return View(updatedRoom);

            var room = await _unitOfWork.Room.GetAsync(r => r.RoomID == id);
            if (room == null)
            {
                return NotFound();
            }

            room.RoomNumber = updatedRoom.RoomNumber;
            room.Capacity = updatedRoom.Capacity;
            room.Status = updatedRoom.Status;
            room.UpdatedAt = DateTime.Now;

            _unitOfWork.Room.Update(room);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _unitOfWork.Room.GetAllAsync();
            var roomList = list.Select(u => new
            {
                u.RoomID,
                u.RoomNumber,
                u.Status,
                u.Capacity,
                u.Theater

            }).ToList();

            return Json(new { data = roomList });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Room room, int cinemaId, int numberOfRows, int seatsPerRow)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid room data." });

            try
            {
                if (!await ValidateRoomCreation(room, numberOfRows, seatsPerRow))
                    return Json(new { success = false, message = "Validation failed." });

                // Tạo phòng
                var newRoom = await CreateRoom(room, cinemaId);

                // Tạo ghế
                await CreateSeats(newRoom.RoomID, numberOfRows, seatsPerRow);

                return Json(new { 
                    success = true, 
                    message = "Room and seats created successfully.",
                    roomId = newRoom.RoomID 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error creating room: {ex.Message}" });
            }
        }

        // Phương thức kiểm tra tính hợp lệ
        private async Task<bool> ValidateRoomCreation(Room room, int numberOfRows, int seatsPerRow)
        {
            // Kiểm tra phòng tồn tại
            if (_unitOfWork.Room.Get(u => u.RoomNumber == room.RoomNumber) != null)
                return false;

            // Kiểm tra số ghế hợp lệ
            int totalSeats = numberOfRows * seatsPerRow;
            if (totalSeats != room.Capacity)
                return false;

            return true;
        }

        // Phương thức tạo phòng
        private async Task<Room> CreateRoom(Room room, int cinemaId)
        {
            room.CreatedAt = DateTime.Now;
            room.UpdatedAt = DateTime.Now;
            room.CinemaID = cinemaId;
            room.Status = RoomStatus.Available;

            _unitOfWork.Room.Add(room);
            await _unitOfWork.SaveAsync();

            return room;
        }

        // Phương thức tạo ghế
        private async Task CreateSeats(int roomId, int numberOfRows, int seatsPerRow)
        {
            var seats = GenerateSeatList(roomId, numberOfRows, seatsPerRow);
            
            foreach (var seat in seats)
            {
                _unitOfWork.Seat.Add(seat);
            }
            await _unitOfWork.SaveAsync();
        }

        // Phương thức tạo danh sách ghế
        private List<Seat> GenerateSeatList(int roomId, int numberOfRows, int seatsPerRow)
        {
            var seats = new List<Seat>();
            char[] rows = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            for (int row = 0; row < numberOfRows; row++)
            {
                for (int seatNum = 1; seatNum <= seatsPerRow; seatNum++)
                {
                    seats.Add(new Seat
                    {
                        RoomID = roomId,
                        Row = row.ToString(),
                        ColumnNumber = seatNum,
                        Status = SeatStatus.Available
                    });
                }
            }

            return seats;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateField(int id, string field, string value)
        {
            var room = await _unitOfWork.Room.GetAsync(u => u.RoomID == id);
            if (room == null)
            {
                return Json(new { success = false, message = "Room not found." });
            }

            try
            {
                switch (field)
                {
                    case "RoomNumber":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Room Number cannot be empty." });
                        }
                        room.RoomNumber = value;
                        break;
                    case "Capacity":
                        if (!int.TryParse(value, out int capacity) || capacity < 1)
                        {
                            return Json(new { success = false, message = "Invalid capacity value." });
                        }
                        room.Capacity = capacity;
                        break;
                    case "Status":
                        if (!Enum.TryParse(value, out RoomStatus status))
                        {
                            return Json(new { success = false, message = "Invalid status value." });
                        }
                        room.Status = status;
                        break;
                    default:
                        return Json(new { success = false, message = "Invalid field." });
                }

                room.UpdatedAt = DateTime.Now;
                _unitOfWork.Room.Update(room);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "Room updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating room: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id)
        {
            var room = await _unitOfWork.Room.GetAsync(r => r.RoomID == id);
            if (room == null)
            {
                return Json(new { success = false, message = "Room not found" });
            }

            try
            {
                room.Status = RoomStatus.Available;
                room.UpdatedAt = DateTime.Now;
                _unitOfWork.Room.Update(room);
                await _unitOfWork.SaveAsync();

                return Json(new { success = true, message = "Room has been locked successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error locking room: {ex.Message}" });
            }
        }
        [HttpPost]
        public IActionResult Unlock(int id)
        {
            var room = _unitOfWork.Room.Get(u => u.RoomID == id);
            if (room == null)
            {
                return Json(new { success = false, message = "Room not found." });
            }

            try
            {
                room.UpdatedAt = DateTime.Now;
                _unitOfWork.Room.Update(room);
                _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "Room unlocked successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error unlocking room: {ex.Message}" });
            }
        }
    }
}
