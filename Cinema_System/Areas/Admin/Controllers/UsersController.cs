using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailService;

        public UsersController(
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
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

        // ✅ CHỈ KIỂM TRA KHI ẤN SAVE
        [HttpPost]
        public async Task<IActionResult> SaveUserChanges([FromBody] ApplicationUser updatedUser)
        {
            if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Id))
                return Json(new { success = false, message = "Invalid user data." });

            var user = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == updatedUser.Id);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            try
            {
                // 🔍 Kiểm tra điều kiện tổng thể
                if (string.IsNullOrWhiteSpace(updatedUser.FullName))
                    return Json(new { success = false, message = "Full name cannot be empty." });

                if (string.IsNullOrWhiteSpace(updatedUser.Email))
                    return Json(new { success = false, message = "Email cannot be empty." });

                if (_unitOfWork.ApplicationUser.Get(u => u.Email == updatedUser.Email && u.Id != updatedUser.Id) != null)
                    return Json(new { success = false, message = "Email already exists." });

                if (string.IsNullOrWhiteSpace(updatedUser.PhoneNumber))
                    return Json(new { success = false, message = "Phone number cannot be empty." });

                if (_unitOfWork.ApplicationUser.Get(u => u.PhoneNumber == updatedUser.PhoneNumber && u.Id != updatedUser.Id) != null)
                    return Json(new { success = false, message = "Phone number already exists." });

                if (!Regex.IsMatch(updatedUser.PhoneNumber, @"^\d{10}$"))
                    return Json(new { success = false, message = "Invalid phone number format." });

                // Không có gì thay đổi
                if (user.FullName == updatedUser.FullName &&
                    user.Email == updatedUser.Email &&
                    user.PhoneNumber == updatedUser.PhoneNumber)
                    return Json(new { success = false, message = "Nothing to change." });

                // ✅ Cập nhật dữ liệu
                user.FullName = updatedUser.FullName.Trim();
                user.Email = updatedUser.Email.Trim();
                user.UserName = updatedUser.Email.Trim();
                user.PhoneNumber = updatedUser.PhoneNumber.Trim();

                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error updating user: {ex.Message}" });
            }
        }

        internal static async Task<IEnumerable<ApplicationUser>> GetUsersByRole(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, string role_Admin)
        {
            var role = await roleManager.FindByNameAsync(role_Admin);
            if (role == null)
            {
                throw new ArgumentException("Role not found.");
            }

            var usersInRole = await userManager.GetUsersInRoleAsync(role_Admin);
            var applicationUsers = usersInRole.Select(user => new ApplicationUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.UserName, // Assuming FullName is stored in UserName
                Role = role_Admin
            });

            return applicationUsers;
        }

        public bool CurrentUser(string idEditing)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine("Current user id: " + userId);
            return userId == idEditing;
            //var user = await _userManager.FindByIdAsync(id);
            //return View(user);
        }
    }
    }

    public class PasswordGenerator
    {
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            return RandomNumberGenerator.GetString(validChars, length);
        }
    }
