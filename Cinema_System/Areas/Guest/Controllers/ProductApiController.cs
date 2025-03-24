using Cinema.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_System.Areas.Guest.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            if (!products.Any())
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm nào." });
            }
            return Ok(products);
        }
    }
}
