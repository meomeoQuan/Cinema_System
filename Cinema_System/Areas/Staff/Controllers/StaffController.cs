using Cinema.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class StaffController : Controller
    {
        public IActionResult CameraScan()
        {
            return View();
        }
    }
}
