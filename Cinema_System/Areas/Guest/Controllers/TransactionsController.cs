using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Cinema_System.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class TransactionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        public TransactionsController(IUnitOfWork unitOfWork,
                                      UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);  

            var transactions = await _unitOfWork.OrderTable.GetAllAsync(
                filter: t => t.Status == OrderStatus.Completed && t.UserID == userId, 
                includeProperties: "OrderDetails,User"
            );

            return View(transactions);  
        }
        public async Task<IActionResult> GetAll()
        {
            var userId = _userManager.GetUserId(User);  

            var transactions = await _unitOfWork.OrderTable.GetAllAsync(
                filter: t => t.Status == OrderStatus.Completed && t.UserID == userId, 
                includeProperties: "OrderDetails,User"
            );

            var transactionList = transactions.Select(t => new
            {
                t.OrderID,
                TotalAmount = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", t.TotalAmount),
                CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                OrderDetails = t.OrderDetails.Select(od => new
                {
                    od.Product.Name,
                    od.Quantity,
                    Price = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", od.Price)
                }).ToList()
            }).ToList();

            return Json(new { data = transactionList }); 
        }

        
    }
}
