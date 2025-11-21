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
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
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
            var orders = await _unitOfWork.OrderTable.GetAllAsync();

            var now = DateTime.Now;
            var startDate = now.AddMonths(-11); // 12 months including current
            var previousStartDate = startDate.AddMonths(-12);
            var previousEndDate = startDate.AddDays(-1);

            // Group and aggregate
            var groupedRevenue = orders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
        {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Amount = g.Sum(o => o.TotalAmount)
                })
                .ToList();

            // Build current 12 months list
            var currentMonths = Enumerable.Range(0, 12)
                .Select(i => startDate.AddMonths(i))
                .ToList();

            // Build previous 12 months list
            var previousMonths = Enumerable.Range(0, 12)
                .Select(i => previousStartDate.AddMonths(i))
                .ToList();

            // Map revenue data
            var currentYearRevenue = currentMonths
                .Select(m => groupedRevenue
                    .FirstOrDefault(r => r.Year == m.Year && r.Month == m.Month)?.Amount ?? 0)
                .ToList();

            var previousYearRevenue = previousMonths
                .Select(m => groupedRevenue
                    .FirstOrDefault(r => r.Year == m.Year && r.Month == m.Month)?.Amount ?? 0)
                .ToList();

            var monthLabels = currentMonths
                .Select(m => m.ToString("MMM yyyy")) // e.g. "Nov 2024"
                .ToList();

            var viewModel = new RevenueViewModel
            {
                MonthlyRevenue = currentYearRevenue,
                LastYearRevenue = previousYearRevenue,
                MonthLabels = monthLabels
            };

            return View(viewModel);
        }



        //public async Task<IActionResult> Revenue()
        //{
        //    // Fetch monthly revenue data from the database
        //    var revenueData = await _unitOfWork.OrderTable.GetAllAsync();

        //    var monthlyRevenue = revenueData
        //        .GroupBy(o => o.CreatedAt.Month)
        //        .Select(g => new { Month = g.Key, Amount = g.Sum(o => o.TotalAmount) })
        //        .OrderBy(r => r.Month)
        //        .Select(r => r.Amount)
        //        .ToList();

        //    if (monthlyRevenue is List<double>)
        //    {
        //        Console.WriteLine("ok");
        //    }
        //    if (monthlyRevenue == null)
        //    {
        //        Console.WriteLine("MonthlyRevenue is null.");
        //    }
        //    // Create the view model
        //    var viewModel = new RevenueViewModel
        //    {
        //        MonthlyRevenue = monthlyRevenue
        //    };


        //    // Pass the view model to the view
        //    return View(viewModel);
        //}


        //public async Task<IActionResult> Index()
        //{
        //    // Giả sử bạn có một phương thức để lấy tổng doanh thu
        //    var revenue = await _unitOfWork.OrderTable.GetTotalRevenueAsync();

        //    // Giả sử bạn có một phương thức để lấy số lượng người dùng
        //    //var userCount = await _unitOfWork.ApplicationUser.GetCountAsync();

        //    // Giả sử bạn có một phương thức để lấy số lượng đơn hàng
        //    //var orderCount = await _unitOfWork.Order.GetCountAsync();

        //    // Tạo một ViewModel để truyền dữ liệu đến view
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