using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
