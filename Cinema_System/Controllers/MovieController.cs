using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Controllers
{
    public class MovieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
