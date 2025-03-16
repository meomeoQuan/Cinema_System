using Microsoft.AspNetCore.Mvc;
using Cinema_System.Areas.Service;
using Cinema_System.Areas.Request;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Models;
using SQLitePCL;
using Cinema.DataAccess.Data;

namespace Cinema_System.Areas
{
    [Area("Guest")]
    [Route("Guest/[controller]/[action]")]
    public class PaymentController : Controller
    {
        private readonly PayOSService _payOSService;
        private readonly PayOS _payOS;
        private readonly ApplicationDbContext _context;

        public PaymentController(PayOSService payOSService, PayOS payOS, ApplicationDbContext context)
        {
            _payOSService = payOSService;
            _payOS = payOS;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            if (request == null || request.TotalAmount <= 0)
            {
                return BadRequest("Invalid payment request.");
            }

            OrderTable order = new OrderTable
            {
                Status = OrderStatus.Pending,
                TotalAmount = request.TotalAmount,
                UserID = "a1234567-b89c-40d4-a123-456789abcdef",
            };

            _context.OrderTables.Add(order);
            await _context.SaveChangesAsync();

            int orderId = order.OrderID;

            // Chuẩn bị danh sách sản phẩm từ Seats & Foods
            var items = new List<ItemData>();

            // Thêm ghế vào danh sách
            foreach (var seat in request.Seats)
            {
                ShowtimeSeat showtimeSeat = _context.showTimeSeats.Find(seat.showTimeSeatId);
                items.Add(new ItemData($"Seat {seat.nameSeat}", 1, (int)showtimeSeat.Price));
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderID = orderId,
                    ShowtimeSeatID = seat.showTimeSeatId,
                    Quantity = 1,
                    Price = showtimeSeat.Price
                });
                await _context.SaveChangesAsync();
            }

            // Thêm thức ăn vào danh sách
            foreach (var food in request.Items)
            {
                items.Add(new ItemData(food.name, food.quantity, food.price));
                _context.OrderDetails.Add(new OrderDetail
                {
                    OrderID = orderId,
                    ProductID = _context.Products.FirstOrDefault(p => p.Name == food.name).ProductID,
                    Quantity = food.quantity,
                    Price = food.price,
                });
                await _context.SaveChangesAsync();

            }

            // Gọi dịch vụ PayOS để tạo thanh toán
            var response = await _payOSService.CreatePaymentAsync(request.TotalAmount, orderId, items, _payOS);

            if (response.error == 0)
            {
                return Json(new { paymentUrl = ((CreatePaymentResult)response.data).checkoutUrl });
            }

            return BadRequest("Payment failed.");
        }


        // Trang hủy
        [HttpGet]
        public IActionResult CancelUrl()
        {
            return View();
        }

        // Trang thành công
        [HttpGet]
        public IActionResult ReturnUrl()
        {
            return View();
        }
    }
}
