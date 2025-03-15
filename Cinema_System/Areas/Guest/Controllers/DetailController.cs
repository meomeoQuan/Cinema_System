using Cinema.DataAccess.Data;
using Cinema.Models; // Import model
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
