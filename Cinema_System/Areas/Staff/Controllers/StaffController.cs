using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class StaffController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        public StaffController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }




        public async Task<IActionResult> ValidAuthentication(int OrderID)
        {

            IEnumerable<OrderDetail> order = await _unitOfWork.OrderDetail.GetAllAsync(u => u.OrderID == OrderID,
            includeProperties: "User,Product,ShowtimeSeat,Order");


            return View(order);
        }

      
        public IActionResult CameraScan()
        {
            return View();
        }
    }
}
