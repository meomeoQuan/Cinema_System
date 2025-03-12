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
    //[Authorize(Roles = SD.Role_Admin)] 
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(IUnitOfWork unitOfWork,
                               UserManager<IdentityUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _unitOfWork.ApplicationUser.GetAllAsync();
            foreach (var user in users)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Guest";
            }
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser updatedUser)
        {
            if (!ModelState.IsValid) return View(updatedUser);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = updatedUser.FullName;
            user.PhoneNumber = updatedUser.PhoneNumber;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _unitOfWork.ApplicationUser.GetAllAsync();
            var usersList = users.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.PhoneNumber,
                u.Role,
                u.LockoutEnd
            }).ToList();

            return Json(new { data = usersList });
        }

        [HttpPost]
        public IActionResult Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_unitOfWork.ApplicationUser.Get(u => u.Email == user.Email) != null)
                    {
                        return Json(new { success = false, message = "Email already exists." });
                    }

                    _unitOfWork.ApplicationUser.Add(user);
                    _unitOfWork.SaveAsync();
                    return Json(new { success = true, message = "User created successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error creating user: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "Invalid user data." });
        }

        [HttpPost]
        public IActionResult UpdateUserField(string id, string field, string value)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            try
            {
                switch (field)
                {
                    case "FullName":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Full Name cannot be empty." });
                        }
                        user.FullName = value;
                        break;
                    case "Email":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Email cannot be empty." });
                        }
                        if (_unitOfWork.ApplicationUser.Get(u => u.Email == value && u.Id != id) != null)
                        {
                            return Json(new { success = false, message = "Email already exists." });
                        }
                        user.Email = value;
                        break;
                    case "PhoneNumber":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            return Json(new { success = false, message = "Phone Number cannot be empty." });
                        }
                        user.PhoneNumber = value;
                        break;
                    default:
                        return Json(new { success = false, message = "Invalid field." });
                }

                _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating user: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Lock(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            try
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
                _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "User locked successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error locking user: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult Unlock(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            try
            {
                user.LockoutEnd = null;
                _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "User unlocked successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error unlocking user: {ex.Message}" });
            }
        }
    }
}



        //    [HttpPost("LockUnlock")]
        //    public async Task<IActionResult> LockUnlock([FromBody] LockUnlockRequest request)
        //    {
        //        var user = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == request.Id);
        //        if (user == null)
        //        {
        //            return Json(new { success = false, message = "User not found." });
        //        }

        //        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
        //        {
        //            // Unlock user
        //            user.LockoutEnd = null;
        //            await _unitOfWork.SaveAsync();
        //            return Json(new { success = true, message = "User unlocked successfully." });
        //        }
        //        else
        //        {
        //            // Lock user for 1 year
        //            user.LockoutEnd = DateTime.Now.AddYears(1);
        //            await _unitOfWork.SaveAsync();
        //            return Json(new { success = true, message = "User locked successfully." });
        //        }
        //    }
        //}

        //public class LockUnlockRequest
        //{
        //    public string Id { get; set; }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Lock(string id)
        //{
        //    if (id == null)
        //    {
        //        return Json(new { success = false, message = "User not found" });
        //    }

        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return Json(new { success = false, message = "User not found" });
        //    }

        //    try
        //    {
        //        // Khóa user bằng cách set LockoutEnd đến tương lai xa
        //        user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        //        var result = await _userManager.UpdateAsync(user);

        //        if (result.Succeeded)
        //        {
        //            return Json(new { success = true, message = "User has been locked successfully" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Failed to lock user" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Unlock(string id)
        //{
        //    if (id == null)
        //    {
        //        return Json(new { success = false, message = "User not found" });
        //    }

        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        return Json(new { success = false, message = "User not found" });
        //    }

        //    try
        //    {
        //        // Mở khóa user bằng cách set LockoutEnd về null
        //        user.LockoutEnd = null;
        //        var result = await _userManager.UpdateAsync(user);

        //        if (result.Succeeded)
        //        {
        //            return Json(new { success = true, message = "User has been unlocked successfully" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, message = "Failed to unlock user" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}


        // public async Task<IActionResult> Details(string id)
        // {
        //     var user = await _userManager.FindByIdAsync(id);
        //     return View(user);
        // }

        // public async Task<IActionResult> Create()
        // {
        //     return View();
        // }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create(ApplicationUser user)
        // {
        //     return View(user);
        // }


