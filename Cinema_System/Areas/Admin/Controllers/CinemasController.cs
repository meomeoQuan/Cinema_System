
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
    //[Authorize(Roles = SD.Role_Admin)] 
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
            var theaters = await _unitOfWork.Cinema.GetAllAsync();
            return View(theaters); 
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

        //[HttpPost]
        //public async Task<IActionResult> Create(ApplicationUser user, string role)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            if (_unitOfWork.ApplicationUser.Get(u => u.Email == user.Email) != null)
        //            {
        //                return Json(new { success = false, message = "Email already exists." });
        //            }
        //            if (_unitOfWork.ApplicationUser.Get(u => u.PhoneNumber == user.PhoneNumber) != null)
        //            {
        //                return Json(new { success = false, message = "Phone number already exists." });
        //            }
        //            if (!IsValidPhoneNumber(user.PhoneNumber ?? string.Empty))
        //            {
        //                return Json(new { success = false, message = "Invalid phone number format." });
        //            }

        //            user.UserName = user.Email; // Ensure UserName is set to Email
        //            string password = GenerateRandomPassword();
        //            var result = await _userManager.CreateAsync(user, password);
        //            if (result.Succeeded)
        //            {
        //                await _userManager.AddToRoleAsync(user, role);
        //                return Json(new { success = true, message = "User created successfully.", password });
        //            }
        //            else
        //            {
        //                return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(new { success = false, message = $"Error creating user: {ex.Message}" });
        //        }
        //    }
        //    return Json(new { success = false, message = "Invalid user data." });
        //}
        

        //[HttpPost]
        //public async Task<IActionResult> UpdateUserField(string id, string field, string value)
        //{
        //    var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
        //    if (user == null)
        //    {
        //        return Json(new { success = false, message = "User not found." });
        //    }

        //    try
        //    {
        //        switch (field)
        //        {
        //            case "FullName":
        //                if (string.IsNullOrWhiteSpace(value))
        //                {
        //                    return Json(new { success = false, message = "Full Name cannot be empty." });
        //                }
        //                user.FullName = value;
        //                break;
        //            case "Email":
        //                if (string.IsNullOrWhiteSpace(value))
        //                {
        //                    return Json(new { success = false, message = "Email cannot be empty." });
        //                }
        //                if (_unitOfWork.ApplicationUser.Get(u => u.Email == value && u.Id != id) != null)
        //                {
        //                    return Json(new { success = false, message = "Email already exists." });
        //                }
        //                user.Email = value;
        //                break;
        //            case "PhoneNumber":
        //                if (string.IsNullOrWhiteSpace(value))
        //                {
        //                    return Json(new { success = false, message = "Phone Number cannot be empty." });
        //                }
        //                if (_unitOfWork.ApplicationUser.Get(u => u.PhoneNumber == value && u.Id != id) != null)
        //                {
        //                    return Json(new { success = false, message = "Phone number already exists." });
        //                }
        //                if (!IsValidPhoneNumber(user.PhoneNumber ?? string.Empty))
        //                {
        //                    return Json(new { success = false, message = "Invalid phone number format." });
        //                }
        //                user.PhoneNumber = value;
        //                break;
        //            default:
        //                return Json(new { success = false, message = "Invalid field." });
        //        }

        //        _ = _unitOfWork.SaveAsync();
        //        return Json(new { success = true, message = "User updated successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = $"Error updating user: {ex.Message}" });
        //    }
        //}

    }
}




// public async Task<IActionResult> Details(string id)
// {
//     var user = await _userManager.FindByIdAsync(id);
//     return View(user);
// }






