using System;
using System.Text.RegularExpressions;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using static QRCoder.PayloadGenerator;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] 
    public class MoviesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MoviesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _unitOfWork.Movie.GetAllAsync();
            return View(list);
        }




        //[HttpGet]
        //public async Task<IActionResult> Create(int cinemaId)
        //{
        //    var cinema = await _unitOfWork.Cinema.GetAsync(c => c.CinemaID == cinemaId);
        //    if (cinema == null)
        //    {
        //        return NotFound();
        //    }
        //    var room = new Room
        //    {
        //        CinemaID = cinemaId,
        //        Cinema = cinema
        //    };
        //    return View(room);
        //}

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Movie movie)
        {
            string validationError = await ValidateMovie(movie);
            if (validationError != null)
            {
                return Json(new { success = false, message = validationError });
            }

            _unitOfWork.Movie.Add(movie);
            await _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Movie created successfully." });
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

        private async Task RemoveSeats(int roomId, int seatsToRemove)
        {
            var seats = await _unitOfWork.Seat.GetAllAsync(s => s.RoomID == roomId);
            if (seats.Count() < seatsToRemove) return;

            var seatsToDelete = seats.OrderByDescending(s => s.SeatName).Take(seatsToRemove);
            _unitOfWork.Seat.RemoveRange(seatsToDelete);

            await _unitOfWork.SaveAsync();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateField(int id, string field, string value)
        {
            var movie = await _unitOfWork.Movie.GetAsync(r => r.MovieID == id);
            if (movie == null)
            {
                return Json(new { success = false, message = "Movie not found." });
            }

            if (string.IsNullOrWhiteSpace(field) || string.IsNullOrWhiteSpace(value))
            {
                return Json(new { success = false, message = "Invalid field or value." });
            }
            switch (field.ToLower())
            {
                case "title":
                    movie.Title = value.Trim();
                    break;
                case "genre":
                    movie.Title = value.Trim();
                    break;
                case "synopsis":
                    movie.Synopsis = value.Trim();
                    break;
                case "trailer":
                    if (!string.IsNullOrWhiteSpace(value) && !Regex.IsMatch(value, @"^(https?://)?(www\.)?(youtube\.com|youtu\.?be)/.+$"))
                    {
                        return Json(new { success = false, message = "Invalid YouTube link." });
                    }
                    movie.TrailerLink = value.Trim();
                    break;
                case "duration":
                    if (!int.TryParse(value, out int duration) || duration <= 0)
                    {
                        return Json(new { success = false, message = "Invalid duration." });
                    }
                    movie.Duration = duration;
                    break;
                case "releasetime":
                    if (!DateTime.TryParse(value, out DateTime releaseDate))
                    {
                        return Json(new { success = false, message = "Invalid release date." });
                    }
                    movie.ReleaseDate = releaseDate;
                    break;
                default:
                    return Json(new { success = false, message = "Invalid field update request." });
            }
  
            _unitOfWork.Movie.Update(movie);
            await _unitOfWork.SaveAsync();
            movie.UpdatedAt = DateTime.Now;

            return Json(new { success = true, message = "Movie updated successfully." });
        }


        private async Task<string> ValidateMovie(Movie movie)
        {
            movie.Title = movie.Title.Trim();
            movie.Genre = movie.Genre.Trim();
            movie.Synopsis = movie.Synopsis?.Trim();
            movie.TrailerLink = movie.TrailerLink?.Trim();
            if (!await CheckLinkAsync(movie.MovieImage, "text/html"))
            {
                return "Invalid Trailer Link";
            }

            if(!await CheckLinkAsync(movie.MovieImage, "image/"))
            {
                return "Invalid Image Link";
            }

            Movie isDuplicate = await _unitOfWork.Movie.GetAsync(r =>
                                                                r.Title == movie.Title &&
                                                                r.Synopsis == movie.Synopsis );


            return null;
        }
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<bool> CheckLinkAsync(string url, string startsWith)
        {
            try 
            {
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var contentType = response.Content.Headers.ContentType.MediaType;
                    return contentType.StartsWith(startsWith);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckRange(object number, object min, object max)
        {
            if (number is IComparable comparableNumber && min is IComparable comparableMin && max is IComparable comparableMax)
            {
                if (comparableNumber.CompareTo(comparableMin) < 0 || comparableNumber.CompareTo(comparableMax) > 0)
                    return false;
                return true;
            }
            throw new ArgumentException("Parameters must be IComparable");
        }
    }
}
