using Cinema.DataAccess.Data;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_System.Areas.Guest.Controllers
{
    [ApiController]
    [Route("api/order-detail")]
    public class OrderDetailApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GetOrderDetails([FromBody] OrderDetailRequest orderDetailsId)
        {
            var showtimeSeats = await _context.OrderDetails
                .Where(s => orderDetailsId.orderDetailsId.Contains(s.OrderDetailID))
                .ToListAsync();

            if (!showtimeSeats.Any())
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm trong đơn hàng này." });
            }

            var orderDetailResponses = new List<OrderDetailResponse>();

            foreach (var orderDetail in showtimeSeats)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == orderDetail.ProductID);
                orderDetailResponses.Add(new OrderDetailResponse
                {
                    orderDetail = orderDetail,
                    product = product
                });
            }

            return Ok(orderDetailResponses);
        }
    }

    public class OrderDetailRequest
    {
        public List<int> orderDetailsId { get; set; } = new List<int>();
    }

    public class OrderDetailResponse
    {
        public OrderDetail orderDetail { get; set; }
        public Product product { get; set; }
    }
}
