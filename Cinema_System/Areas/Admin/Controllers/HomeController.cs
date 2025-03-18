
﻿using Cinema.DataAccess.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Cinema_System.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")] 
    //[Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext context)
        {
            _db = context;
        }
        [HttpGet]
        public IActionResult GetMonthlyRevenue()
        {
            var revenueData = _db.OrderTables
                .GroupBy(o => o.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Amount = g.Sum(o => o.TotalAmount) })
                .OrderBy(r => r.Month)
                .Select(r => r.Amount)
                .ToArray();
            return Ok(revenueData);
        }
    }
}
