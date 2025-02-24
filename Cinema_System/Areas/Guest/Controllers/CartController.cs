using Microsoft.AspNetCore.Mvc;


namespace Cinema.Controllers
{
    [Area("Guest")]
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View("Cart");
        }
        public IActionResult Detail()
        {
            return View("../Home/Details");
        }
    }
}
