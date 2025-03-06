using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class DetailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
