using Cinema.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Cinema_System.Areas.Request;
using Microsoft.EntityFrameworkCore;
using Cinema.Models;

namespace Cinema_System.Areas.Guest.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] PaymentWebhookRequest request)
        {
            try
            {
                // Tìm đơn hàng theo orderCode
                var order = await _context.OrderTables.FirstOrDefaultAsync(o => o.OrderID == request.OrderCode);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                // Kiểm tra trạng thái thanh toán từ PayOS
                if (request.Status == "PAID")
                {
                    order.Status = OrderStatus.Completed;  // Cập nhật trạng thái đơn hàng
                    order.UpdatedAt = DateTime.UtcNow; // Ghi nhận thời gian thanh toán
                    await _context.SaveChangesAsync(); // Lưu vào database
                }

                return Ok(new { message = "Payment updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error updating payment", error = ex.Message });
            }
        }
    }

}
