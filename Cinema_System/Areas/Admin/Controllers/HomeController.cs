using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Models.ViewModels;
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
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork,
                              ApplicationDbContext context)
        {
            _db = context;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        // [HttpGet]
        // public IActionResult GetMonthlyRevenue()
        // {
        //     var revenueData = _db.OrderTables
        //         .GroupBy(o => o.CreatedAt.Month)
        //         .Select(g => new { Month = g.Key, Amount = g.Sum(o => o.TotalAmount) })
        //         .OrderBy(r => r.Month)
        //         .Select(r => r.Amount)
        //         .ToArray();
        //     return Ok(revenueData);
        // }



        public async Task<IActionResult> Revenue()
        {
            // Fetch monthly revenue data from the database
            var revenueData = await _unitOfWork.OrderTable.GetAllAsync();
            var monthlyRevenue = revenueData
                .GroupBy(o => o.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Amount = g.Sum(o => o.TotalAmount) })
                .OrderBy(r => r.Month)
                .Select(r => r.Amount)
                .ToList();

            // Create the view model
            var viewModel = new RevenueViewModel
            {
                MonthlyRevenue = monthlyRevenue
            };

            // Pass the view model to the view
            return View(viewModel);
        }


        //public async Task<IActionResult> Index()
        //{
        //    // Gi? s? b?n c� m?t ph??ng th?c ?? l?y t?ng doanh thu
        //    var revenue = await _unitOfWork.OrderTable.GetTotalRevenueAsync();

        //    // Gi? s? b?n c� m?t ph??ng th?c ?? l?y s? l??ng ng??i d�ng
        //    //var userCount = await _unitOfWork.ApplicationUser.GetCountAsync();

        //    // Gi? s? b?n c� m?t ph??ng th?c ?? l?y s? l??ng ??n h�ng
        //    //var orderCount = await _unitOfWork.Order.GetCountAsync();

        //    // T?o m?t ViewModel ?? truy?n d? li?u ??n view
        //    var dashboardViewModel = new DashboardViewModel
        //    {
        //        TotalRevenue = revenue

        //        //UserCount = userCount,
        //        //OrderCount = orderCount
        //    };

        //    return View(dashboardViewModel);
        //}
    }
}