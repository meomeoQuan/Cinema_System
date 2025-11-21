using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Generators;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static QRCoder.PayloadGenerator;

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
                // dung lo em default neu ma luc 
                // create user ko set role thi no se la guest -- quan 
            }

            ViewBag.RolesList = _roleManager.Roles
                                    .Where(r => r.Name != SD.Role_Guest) // Lọc bỏ vai trò Guest nếu không muốn gán
                                    .Select(r => r.Name)
                                    .ToList();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user, string role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_unitOfWork.ApplicationUser.Get(u => u.Email == user.Email) != null)
                    {
                        return Json(new { success = false, message = "Email already exists. hehe" });
                    }
                    if (_unitOfWork.ApplicationUser.Get(u => u.PhoneNumber == user.PhoneNumber) != null)
                    {
                        return Json(new { success = false, message = "Phone number already exists." });
                    }
                    if (!IsValidPhoneNumber(user.PhoneNumber ?? string.Empty))
                    {
                        return Json(new { success = false, message = "Invalid phone number format." });
                    }

                    user.UserName = user.Email; // Ensure UserName is set to Email
                    string password = PasswordGenerator.GenerateRandomPassword();
                    user.EmailConfirmed = true;
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await _emailService.SendEmailAsync(
                            user.Email,
                            "Create User Account Successfully",
                            $"<p>Hi {user.FullName}!</p><p>Your password is: {password}</p><p>Please change your password after logging in for the first time.</p>"
                        );

                        await _userManager.AddToRoleAsync(user, role);
                        return Json(new { success = true, message = "User created successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error creating user: {ex.Message}" });
                }
            }
            return Json(new { success = false, message = "Invalid user data." });
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Define a regular expression for validating phone numbers
            var phoneRegex = new Regex(@"^\d{10}$"); // Example: 10-digit phone number
            return phoneRegex.IsMatch(phoneNumber);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // ✅ BẢO MẬT: Thêm AntiForgeryToken
        public async Task<IActionResult> SaveUserChanges([FromBody] ApplicationUser updatedUser)
        {
            if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Id))
                return Json(new { success = false, message = "Invalid user data." });

            // Lấy người dùng từ UserManager để đảm bảo tính toàn vẹn
            var user = await _userManager.FindByIdAsync(updatedUser.Id) as ApplicationUser;
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            try
            {
                // Không có gì thay đổi
                if (user.FullName == updatedUser.FullName &&
                    user.Email == updatedUser.Email &&
                    user.PhoneNumber == updatedUser.PhoneNumber &&
                    user.Role == updatedUser.Role)
                    return Json(new { success = true, message = "No changes were detected." });

                // 🔍 Kiểm tra điều kiện tổng thể
                if (string.IsNullOrWhiteSpace(updatedUser.FullName))
                    return Json(new { success = false, message = "Full name cannot be empty." });

                if (string.IsNullOrWhiteSpace(updatedUser.Email))
                    return Json(new { success = false, message = "Email cannot be empty." });

                // Tối ưu: Sử dụng AnyAsync để kiểm tra sự tồn tại
                try
                {
                    var addr = new MailAddress(updatedUser.Email);
                    if (addr.Address != updatedUser.Email.Trim())
                        return Json(new { success = false, message = "Invalid email format." });
                }
                catch
                {
                    return Json(new { success = false, message = "Invalid email format." });
                }

                if (await _unitOfWork.ApplicationUser.AnyAsync(u => u.Email == updatedUser.Email && u.Id != updatedUser.Id))
                    return Json(new { success = false, message = "This email already exists." });

                if (string.IsNullOrWhiteSpace(updatedUser.PhoneNumber))
                    return Json(new { success = false, message = "Phone number cannot be empty." });

                if (!IsValidPhoneNumber(updatedUser.PhoneNumber))
                    return Json(new { success = false, message = "Phone number must be a 10-digit number." });

                if (await _unitOfWork.ApplicationUser.AnyAsync(u => u.PhoneNumber == updatedUser.PhoneNumber && u.Id != updatedUser.Id))
                    return Json(new { success = false, message = "This phone number already exists." });


                // ✅ Cập nhật dữ liệu thông qua UserManager
                user.FullName = updatedUser.FullName.Trim();
                user.PhoneNumber = updatedUser.PhoneNumber.Trim();

                // Cập nhật email và username một cách an toàn
                await _userManager.SetEmailAsync(user, updatedUser.Email.Trim());
                await _userManager.SetUserNameAsync(user, updatedUser.Email.Trim());

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "User updated successfully." });
                }

                return Json(new { success = false, message = "Error when updating user: " + string.Join(", ", result.Errors.Select(e => e.Description)) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (id == currentUserId)
            {
                return Json(new { success = false, message = "You cannot lock your own account." });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Dùng SetLockoutEndDateAsync là cách làm đúng chuẩn của Identity
            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

            if (result.Succeeded)
            {
                return Json(new { success = true, message = "User locked successfully." });
            }

            return Json(new { success = false, message = "Failed to lock user." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Dùng SetLockoutEndDateAsync để mở khóa
            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (result.Succeeded)
            {
                return Json(new { success = true, message = "User unlocked successfully." });
            }

            return Json(new { success = false, message = "Failed to unlock user." });
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
    //public static string GenerateRandomPassword(int length = 12)
    //{
    //    const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
    //    StringBuilder result = new StringBuilder(length);
    //    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    //    {
    //        byte[] uintBuffer = new byte[sizeof(uint)];

    //        while (length-- > 0)
    //        {
    //            rng.GetBytes(uintBuffer);
    //            uint num = BitConverter.ToUInt32(uintBuffer, 0);
    //            result.Append(validChars[(int)(num % (uint)validChars.Length)]);
    //        }
    //    }
    //    return result.ToString();
    //}
    //public static string GenerateRandomPassword(int length = 12)
    //{
    //    const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
    //    return RandomNumberGenerator.GetString(validChars, length);
    //}

    public static string GenerateRandomPassword()
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
        var random = new Random();
        var password = new StringBuilder();
        for (int i = 0; i < 12; i++)
        {
            password.Append(validChars[random.Next(validChars.Length)]);
        }
        return password.ToString();
    }
}