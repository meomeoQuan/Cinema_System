
﻿using System.Threading.Tasks;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]

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

            // Lấy danh sách rạp chiếu phim
            var cinemas = await _unitOfWork.Cinema
                                //.Include(t => t.Admin)
                                .GetAllAsync(includeProperties: "Admin");

            // Lấy danh sách admin và gán vào ViewBag
            var admins = await UsersController.GetUsersByRole(_userManager, _roleManager, SD.Role_Admin);

            ViewBag.Admins = admins.Select(a => new { Id = a.Id, FullName = a.FullName, Role = a.Role }).ToList();

            return View(cinemas);
        }

        public async Task<IActionResult> Create(Theater theater)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra tên rạp đã tồn tại chưa
                    if (_unitOfWork.Cinema.Get(c => c.Name == theater.Name) != null)
                    {
                        return Json(new { success = false, message = "Theater name already exists." });
                    }

                    // Kiểm tra địa chỉ rạp đã tồn tại chưa
                    if (_unitOfWork.Cinema.Get(c => c.Address == theater.Address) != null)
                    {
                        return Json(new { success = false, message = "Theater address already exists." });
                    }

                    // Kiểm tra số phòng hợp lệ
                    if (theater.NumberOfRooms <= 0)
                    {
                        return Json(new { success = false, message = "Number of rooms must be greater than 0." });
                    }

                    // Thêm rạp chiếu phim vào database
                    _unitOfWork.Cinema.Add(theater);
                    await _unitOfWork.SaveAsync();

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

                AdminName = c.Admin?.FullName ?? "Unknown",
                c.NumberOfRooms,
                c.OpeningTime,
                c.ClosingTime

            }).ToList();

            return Json(new { data = cinemasList });
        }

    }
}

