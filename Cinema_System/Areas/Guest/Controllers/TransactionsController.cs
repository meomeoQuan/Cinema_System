using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class TransactionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /Admin/Transactions/Index
        public async Task<IActionResult> Index()
        {
            var transactions = await _unitOfWork.OrderTable.GetAllAsync(
                filter: t => t.Status == OrderStatus.Completed, // Chỉ lấy đơn hàng có trạng thái "Completed"
                includeProperties: "OrderDetails,User"
            );
            return View(transactions);
        }

        // GET: /Admin/Transactions/GetAll
        public async Task<IActionResult> GetAll()
        {
            var transactions = await _unitOfWork.OrderTable.GetAllAsync(
                filter: t => t.Status == OrderStatus.Completed, // Chỉ lấy đơn hàng hoàn thành
                includeProperties: "OrderDetails,User"
            );

            var transactionList = transactions.Select(t => new
            {
                t.OrderID,
                UserName = t.User?.FullName ?? "Unknown",
                TotalAmount = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", t.TotalAmount), // Hiển thị tiền tệ VND
                CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                OrderDetails = t.OrderDetails.Select(od => new
                {
                    od.Product.Name,
                    od.Quantity,
                    Price = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", od.Price) // Hiển thị giá VND
                }).ToList()
            }).ToList();

            return Json(new { data = transactionList });
        }

        // GET: /Admin/Transactions/GetOrderDetails
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid Order ID." });
            }

            var order = (await _unitOfWork.OrderTable.GetAllAsync(
                filter: o => o.OrderID == orderId && o.Status == OrderStatus.Completed, // Chỉ lấy đơn hàng hoàn thành
                includeProperties: "OrderDetails,OrderDetails.Product")).FirstOrDefault();

            if (order == null)
            {
                return NotFound(new { success = false, message = "Order not found or not completed." });
            }

            var orderDetails = order.OrderDetails.Select(od => new
            {
                ProductName = od.Product?.Name ?? "Unknown Product",
                od.Quantity,
                Price = string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:C0}", od.Price) // Hiển thị giá VND
            }).ToList();

            return Json(new { success = true, data = orderDetails });
        }
    }
}
