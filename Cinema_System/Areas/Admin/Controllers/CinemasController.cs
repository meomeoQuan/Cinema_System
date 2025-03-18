
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
    //[Authorize(Roles = "Admin")]
    public class CinemasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CinemasController(IUnitOfWork unitOfWork,
                               UserManager<IdentityUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var cinemas = await _unitOfWork.Cinema.GetAllAsync();
            return View(cinemas); 
        }

        public async Task<IActionResult> Create(Theater theater)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine($"Received Theater: Name={theater.Name}, Status={theater.Status}");

                    // Check if Theater Name already exists
                    if (_unitOfWork.Cinema.Get(c => c.Name == theater.Name) != null)
                    {
                        return Json(new { success = false, message = "Theater name already exists." });
                    }

                    // Check if Address already exists
                    if (_unitOfWork.Cinema.Get(c => c.Address == theater.Address) != null)
                    {
                        return Json(new { success = false, message = "Theater address already exists." });
                    }

                    // Validate Number of Rooms
                    if (theater.NumberOfRooms <= 0)
                    {
                        return Json(new { success = false, message = "Number of rooms must be greater than 0." });
                    }


                    Console.WriteLine($"Saving Theater with Status: {theater.Status}");

                    // Add Theater to Database
                    _unitOfWork.Cinema.Add(theater);

                    // Save changes
                    await _unitOfWork.SaveAsync();

                    Console.WriteLine("Theater saved successfully.");

                    return Json(new { success = true, message = "Theater created successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error creating theater: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "Invalid theater data." });
        }



        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var cinemas = await _unitOfWork.Cinema.GetAllAsync();
            var cinemasList = cinemas.Select(c => new
            {
                c.CinemaID,
                c.Name,
                c.Address,
                c.AdminID,
                c.NumberOfRooms,
                c.OpeningTime,
                c.ClosingTime
            }).ToList();

            return Json(new { data = cinemasList });
        }
    
    }
   
}








