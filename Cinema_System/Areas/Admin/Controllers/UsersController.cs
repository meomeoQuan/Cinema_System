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

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser user)
        {
            return View(user);
        }


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
    }
}