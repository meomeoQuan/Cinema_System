using Cinema.DataAccess.Data;
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


        //[HttpGet]
        //public IActionResult GetMonthlyRevenue()
        //{
        //    var revenueData = new[] { 12000, 15000, 10000, 20000, 25000, 30000, 35000, 40000, 45000, 50000, 55000, 60000 };
        //    return Ok(revenueData);
        //}
    }
}


