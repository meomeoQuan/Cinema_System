using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
