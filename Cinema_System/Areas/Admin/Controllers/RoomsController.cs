using System.Security.Claims;
using System.Security.Cryptography;
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

        public RoomsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int cinemaId)
        {
            if (cinemaId <= 0)
            {
                return BadRequest("Invalid cinema ID");
            }

            ViewData["CinemaId"] = cinemaId;

            var list = await _unitOfWork.Room.GetAllAsync(r => r.CinemaID == cinemaId, includeProperties: "Theater");
            
            return View(list);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetRoomsByCinema(int cinemaId)
        //{
        //    if (cinemaId <= 0)
        //    {
        //        return Json(new { success = false, message = "Invalid cinema ID" });
        //    }

        //    try
        //    {
        //        var rooms = await _unitOfWork.Room.GetAllAsync(r => r.CinemaID == cinemaId);

        //        if (rooms == null || !rooms.Any())
        //        {
        //            return Json(new { success = false, message = "No rooms found", rooms = new List<object>() });
        //        }

        //        return Json(new { success = true, rooms = rooms.Select(r => new { r.RoomID, r.RoomNumber }) });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Error retrieving rooms: {ex.Message}" });
        //    }
        //}

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

        //[HttpGet("GetAll")]
        //public async Task<IActionResult> GetAll()
        //{
        //    var list = await _unitOfWork.Room.GetAllAsync();
        //    var roomList = list.Select(u => new
        //    {
        //        u.RoomID,
        //        u.RoomNumber,
        //        u.Status,
        //        u.Capacity,
        //        u.Theater

        //    }).ToList();

        //    return Json(new { data = roomList });
        //}

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Room room)
        {
            string validationError = await ValidateRoom(room);
            if (validationError != null)
            {
                return Json(new { success = false, message = validationError });
            }

            try
            {
                var newRoom = await CreateRoom(room, room.CinemaID);
                await CreateSeats(newRoom.RoomID, room.Capacity / 10, 10);
                return Json(new { success = true, message = "Room created successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error creating room: {ex.Message}" });
            }
        }

        private async Task<string> ValidateRoomCreation(Room room, int numberOfRows, int seatsPerRow)
        {
            // Check if the room number already exists
            var existingRoom = _unitOfWork.Room.Get(u => u.RoomNumber == room.RoomNumber);
            if (existingRoom != null)
            {
                return "Room number already exists. Please choose another.";
            }

            // Ensure the total number of seats matches capacity
            int totalSeats = numberOfRows * seatsPerRow;
            if (totalSeats != room.Capacity)
            {
                return $"Invalid capacity. Expected {totalSeats} seats based on row configuration, but got {room.Capacity}.";
            }

            return null; // No validation errors
        }


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

        
        private List<Seat> GenerateSeatList(int roomId, int numberOfRows, int seatsPerRow)
        {
            var seats = new List<Seat>();

            for (int row = 0; row < numberOfRows; row++)
            {
                string rowLabel = ConvertToLetter(row); // Convert index to A-Z, AA, AB, etc.

                for (int seatNum = 1; seatNum <= seatsPerRow; seatNum++)
                {
                    seats.Add(new Seat
                    {
                        RoomID = roomId,
                        Row = rowLabel,
                        ColumnNumber = seatNum,
                        Status = SeatStatus.Available
                    });
                }
            }

            return seats;
        }

        private async Task AddSeats(int roomId, int seatsToAdd)
        {
            var room = await _unitOfWork.Room.GetAsync(r => r.RoomID == roomId);
            if (room == null) return;

            var existingSeats = await _unitOfWork.Seat.GetAllAsync(s => s.RoomID == roomId);
            int currentSeatCount = existingSeats.Count();

            int newCapacity = currentSeatCount + seatsToAdd;
            int numberOfRows = newCapacity / 10; // Example: If 20 seats → 2 rows
            int seatsPerRow = 10;

            var newSeats = GenerateSeatList(roomId, numberOfRows, seatsPerRow);

            var seatsToInsert = newSeats.Where(s => !existingSeats.Any(es => es.Row == s.Row && es.ColumnNumber == s.ColumnNumber)).ToList();

            foreach (var seat in seatsToInsert)
            {
                _unitOfWork.Seat.Add(seat);
            }

            await _unitOfWork.SaveAsync();
        }



        private async Task RemoveSeats(int roomId, int seatsToRemove)
        {
            var seats = await _unitOfWork.Seat.GetAllAsync(s => s.RoomID == roomId);
            if (seats.Count() < seatsToRemove) return;

            var seatsToDelete = seats.OrderByDescending(s => s.SeatName).Take(seatsToRemove);
            _unitOfWork.Seat.RemoveRange(seatsToDelete);

            await _unitOfWork.SaveAsync();
        }


        private string ConvertToLetter(int index)
        {
            string result = "";
            while (index >= 0)
            {
                result = (char)('A' + (index % 26)) + result;
                index = (index / 26) - 1;
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateField(int id, string field, string value)
        {
            var room = await _unitOfWork.Room.GetAsync(r => r.RoomID == id);
            if (room == null)
            {
                return Json(new { success = false, message = "Room not found." });
            }

            if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(value))
            {
                return Json(new { success = false, message = "Invalid field or value." });
            }

            if (field == "RoomNumber")
            {
                room.RoomNumber = value.Trim();
            }
            else if (field == "Capacity")
            {
                if (!int.TryParse(value, out int capacity) || capacity < 10 || capacity % 10 != 0)
                {
                    return Json(new { success = false, message = "Capacity must be at least 10 and a multiple of 10." });
                }

                int oldCapacity = room.Capacity;
                room.Capacity = capacity;

                try
                {
                    // ✅ Update the room capacity in the database first
                    _unitOfWork.Room.Update(room);
                    await _unitOfWork.SaveAsync();

                    // ✅ Get the current seat count
                    var existingSeats = await _unitOfWork.Seat.GetAllAsync(s => s.RoomID == room.RoomID);
                    int currentSeatCount = existingSeats.Count();

                    if (capacity > currentSeatCount)
                    {
                        // 🟢 Add missing seats
                        int seatsToAdd = capacity - currentSeatCount;
                        await AddSeats(room.RoomID, seatsToAdd);
                    }
                    else if (capacity < currentSeatCount)
                    {
                        // 🔴 Only remove extra seats (do not remove all if they already match)
                        int seatsToRemove = currentSeatCount - capacity;
                        if (seatsToRemove > 0)
                        {
                            await RemoveSeats(room.RoomID, seatsToRemove);
                        }
                    }

                    return Json(new { success = true, message = "Room updated successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error updating room: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Invalid field update request." });
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
                room.Status = RoomStatus.Maintenance;
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
                room.Status = RoomStatus.Available;
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


        private async Task<string> ValidateRoom(Room room, bool isUpdate = false)
        {
            room.RoomNumber = room.RoomNumber.Trim();

            if (string.IsNullOrWhiteSpace(room.RoomNumber) || !Regex.IsMatch(room.RoomNumber, @"^[a-zA-Z0-9\s]+$"))
            {
                return "Invalid Room Number. Only letters, numbers, and spaces are allowed.";
            }

            Room isDuplicate = await _unitOfWork.Room.GetAsync(r =>
                                                                r.CinemaID == room.CinemaID &&
                                                                r.RoomNumber == room.RoomNumber &&
                                                                (!isUpdate || r.RoomID != room.RoomID));

            if (isDuplicate != null)
            {
                return "Room Number already exists. Please use a unique number.";
            }

            if (room.Capacity < 10 || room.Capacity % 10 != 0)
            {
                return "Capacity must be at least 10 and a multiple of 10.";
            }

            var existingCinema = await _unitOfWork.Cinema.GetAsync(c => c.CinemaID == room.CinemaID, includeProperties: "Rooms");
            if (existingCinema != null && existingCinema.Rooms.Count >= existingCinema.NumberOfRooms)
            {
                return $"Cannot add more rooms. Cinema can only have {existingCinema.NumberOfRooms} rooms.";
            }

            return null; 
        }


    }
}
