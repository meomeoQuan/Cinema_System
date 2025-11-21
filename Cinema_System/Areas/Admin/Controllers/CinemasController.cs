using System.Threading.Tasks;
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
                                .GetAllAsync(includeProperties: "Admin");

            // Lấy danh sách admin và gán vào ViewBag
            var admins = await UsersController.GetUsersByRole(_userManager, _roleManager, SD.Role_Admin);

            //ViewBag.Admins = admins.Select(a => new { Id = a.Id, FullName = a.FullName, Role = a.Role }).ToList();
            ViewBag.Admins = admins.Select(a => new { a.Id, a.FullName }).ToList();

            return View(cinemas);
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

        public async Task<IActionResult> Create(Theater theater)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_unitOfWork.Cinema.Get(c => c.Name == theater.Name) != null)
                    {
                        return Json(new { success = false, message = "Theater name already exists." });
                    }
                    if (_unitOfWork.Cinema.Get(c => c.Address == theater.Address) != null)
                    {
                        return Json(new { success = false, message = "Theater address already exists." });
                    }
                    if (theater.NumberOfRooms <= 0)
                    {
                        return Json(new { success = false, message = "Number of rooms must be greater than 0." });
                    }
                    //if (theater.OpeningTime >= theater.ClosingTime)
                    //{
                    //    return Json(new { success = false, message = "Closing Time must be later than Opening Time." });
                    //}
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

        [HttpPost]
        public async Task<IActionResult> UpdateTheater(Theater updatedTheater)
        {
            if (updatedTheater == null || updatedTheater.CinemaID == 0)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var theaterFromDb = await _unitOfWork.Cinema.GetAsync(c => c.CinemaID == updatedTheater.CinemaID);
            if (theaterFromDb == null)
            {
                return Json(new { success = false, message = "Theater not found." });
            }

            // --- SERVER-SIDE VALIDATION ---
            if (string.IsNullOrWhiteSpace(updatedTheater.Name))
            {
                return Json(new { success = false, message = "Theater name cannot be empty." });
            }
            if (await _unitOfWork.Cinema.AnyAsync(c => c.Name == updatedTheater.Name && c.CinemaID != updatedTheater.CinemaID))
            {
                return Json(new { success = false, message = "Theater name already exists." });
            }
            if (string.IsNullOrWhiteSpace(updatedTheater.Address))
            {
                return Json(new { success = false, message = "Address cannot be empty." });
            }
            if (await _unitOfWork.Cinema.AnyAsync(c => c.Address == updatedTheater.Address && c.CinemaID != updatedTheater.CinemaID))
            {
                return Json(new { success = false, message = "Theater address already exists." });
            }
            if (updatedTheater.NumberOfRooms < 1)
            {
                return Json(new { success = false, message = "Number of rooms must be at least 1." });
            }
            //if (updatedTheater.OpeningTime >= updatedTheater.ClosingTime)
            //{
            //    return Json(new { success = false, message = "Closing Time must be later than Opening Time." });
            //}

            // Cập nhật các trường
            theaterFromDb.Name = updatedTheater.Name;
            theaterFromDb.Address = updatedTheater.Address;
            theaterFromDb.NumberOfRooms = updatedTheater.NumberOfRooms;
            theaterFromDb.OpeningTime = updatedTheater.OpeningTime;
            theaterFromDb.ClosingTime = updatedTheater.ClosingTime;
            // Xử lý AdminID có thể null hoặc rỗng
            theaterFromDb.AdminID = string.IsNullOrEmpty(updatedTheater.AdminID) ? null : updatedTheater.AdminID;
            theaterFromDb.UpdatedAt = DateTime.Now;

            try
            {
                _unitOfWork.Cinema.Update(theaterFromDb);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "Theater updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating theater: {ex.Message}" });
            }
        }

        public async Task<IActionResult> ToggleCinemaStatus(int id)
        {
            var cinema = await _unitOfWork.Cinema.GetAsync(c => c.CinemaID == id);
            if (cinema == null)
            {
                return Json(new { success = false, message = "Cinema not found." });
            }

            cinema.Status = cinema.Status == CinemaStatus.Open ? CinemaStatus.Closed : CinemaStatus.Open;
            cinema.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = $"Cinema is now {(cinema.Status == CinemaStatus.Open ? "Open" : "Closed")}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating cinema status: {ex.Message}" });
            }
        }
    }
}