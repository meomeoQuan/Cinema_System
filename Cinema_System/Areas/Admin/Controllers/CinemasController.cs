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

        [HttpPost]
        public async Task<IActionResult> UpdateTheaterField(int id, string field, string value)
        {
            var theater = _unitOfWork.Cinema.Get(c => c.CinemaID == id);
            if (theater == null)
            {
                return Json(new { success = false, message = "Theater not found." });
            }

            try
            {
                switch (field)
                {
                    case "Name":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Theater name cannot be empty." });
                        }
                        if (_unitOfWork.Cinema.Get(c => c.Name == value && c.CinemaID != id) != null)
                        {
                            return Json(new { success = false, message = "Theater name already exists." });
                        }
                        theater.Name = value;
                        break;

                    case "Address":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Address cannot be empty." });
                        }
                        if (_unitOfWork.Cinema.Get(c => c.Address == value && c.CinemaID != id) != null)
                        {
                            return Json(new { success = false, message = "Theater address already exists." });
                        }
                        theater.Address = value;
                        break;

                    case "CinemaCity":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Cinema city cannot be empty." });
                        }
                        theater.CinemaCity = value;
                        break;

                    case "NumberOfRooms":
                        if (!int.TryParse(value, out int numRooms) || numRooms < 1)
                        {
                            return Json(new { success = false, message = "Number of rooms must be at least 1." });
                        }
                        theater.NumberOfRooms = numRooms;
                        break;

                    case "Status":
                        if (!Enum.TryParse(value, true, out CinemaStatus status))
                        {
                            return Json(new { success = false, message = "Invalid cinema status." });
                        }
                        theater.Status = status;
                        break;

                    case "OpeningTime":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Opening time cannot be empty." });
                        }
                        theater.OpeningTime = value;
                        break;

                    case "ClosingTime":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Closing time cannot be empty." });
                        }
                        theater.ClosingTime = value;
                        break;

                    case "AdminID":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Admin ID cannot be empty." });
                        }
                        var admin = _unitOfWork.ApplicationUser.Get(a => a.Id == value);
                        if (admin == null)
                        {
                            return Json(new { success = false, message = "Admin not found." });
                        }
                        theater.AdminID = value;
                        break;

                    default:
                        return Json(new { success = false, message = "Invalid field." });
                }

                theater.UpdatedAt = DateTime.Now; // Cập nhật thời gian chỉnh sửa
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