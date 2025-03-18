using System.Security.Claims;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
   
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ShoppingCartVM = new()
            {
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.UserID == userId, includeProperties: "Product,ShowtimeSeat.Seat"),
                OrderHeader = new OrderTable()
            };
            //foreach (var cart in ShoppingCartVM.ListCart)
            //{
            //    cart.Price = CalculateOrderTotal(cart);
            //    ShoppingCartVM.OrderHeader.TotalAmount += cart.Price * cart.Quantity;
            //}

            return View(ShoppingCartVM);
        }

        //public IActionResult Plus(int cartId)
        //{
        //    var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ProductId == cartId);
        //    cartFromDb.Count += 1;
        //    _unitOfWork.ShoppingCart.Update(cartFromDb);
        //    _unitOfWork.Save();
        //    return RedirectToAction(nameof(Index));

        //}

        //public IActionResult Subtract(int cartId)
        //{
        //    var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ProductId == cartId);

        //    if (cartFromDb.Count <= 1)
        //    {
        //        HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
        //        _unitOfWork.ShoppingCart.Remove(cartFromDb);

        //    }
        //    else
        //    {
        //        cartFromDb.Count -= 1;
        //        _unitOfWork.ShoppingCart.Update(cartFromDb);


        //    }
        //    _unitOfWork.Save();
        //    return RedirectToAction(nameof(Index));
        //}

        //public IActionResult DeleteItem(int cartId)
        //{
        //    var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ProductId == cartId);
        //    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
        //    _unitOfWork.ShoppingCart.Remove(cartFromDb);
        //    _unitOfWork.Save();
        //    return RedirectToAction(nameof(Index));

        //}

        //public IActionResult Summary()
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    ShoppingCartVM = new()
        //    {
        //        ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
        //        OrderHeader = new OrderHeader()
        //    };

        //    ShoppingCartVM.OrderHeader.applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        //    // to populate the orderHeader with the user details from the applicationUser table
        //    ShoppingCartVM.OrderHeader.name = ShoppingCartVM.OrderHeader.applicationUser.name;
        //    ShoppingCartVM.OrderHeader.phoneNumber = ShoppingCartVM.OrderHeader.applicationUser.PhoneNumber;
        //    ShoppingCartVM.OrderHeader.streetAddress = ShoppingCartVM.OrderHeader.applicationUser.streetAddress;
        //    ShoppingCartVM.OrderHeader.city = ShoppingCartVM.OrderHeader.applicationUser.city;
        //    ShoppingCartVM.OrderHeader.state = ShoppingCartVM.OrderHeader.applicationUser.state;
        //    ShoppingCartVM.OrderHeader.portalCode = ShoppingCartVM.OrderHeader.applicationUser.portalCode;

        //    foreach (var cart in ShoppingCartVM.ListCart)
        //    {
        //        cart.Price = CalculateOrderTotal(cart);
        //        ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        //    }
        //    return View(ShoppingCartVM);
        //}


        //[HttpPost]
        //[ActionName("Summary")]
        //public IActionResult SummaryPost()
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product");

        //    ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
        //    ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

        //    // this important
        //    ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        //    // to populate the orderHeader with the user details from the applicationUser table


        //    foreach (var cart in ShoppingCartVM.ListCart)
        //    {
        //        cart.Price = CalculateOrderTotal(cart);
        //        ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        //    }

        //    if (applicationUser.CompanyID.GetValueOrDefault() == 0)
        //    {
        //        // regular customer
        //        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        //        ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
        //    }
        //    else
        //    {
        //        // for company customers
        //        ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
        //        ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
        //    }
        //    _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
        //    _unitOfWork.Save();

        //    foreach (var cart in ShoppingCartVM.ListCart)
        //    {
        //        OrderDetail orderDetail = new()
        //        {
        //            ProductId = cart.ProductId,
        //            OrderHeaderId = ShoppingCartVM.OrderHeader.OrderHeaderId,
        //            Price = cart.Price,
        //            Count = cart.Count
        //        };
        //        _unitOfWork.OrderDetails.Add(orderDetail);
        //        _unitOfWork.Save();
        //    }

        //    return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.OrderHeaderId });
        //}

        //public IActionResult OrderConfirmation(int id)
        //{
        //    // id is orderheader id
        //    //HttpContext.Session.Clear();
        //    return View(id);
        //}

        //private double CalculateOrderTotal(ShoppingCart shoppingCart)
        //{
        //    if (shoppingCart.Quantity <= 50)
        //    {
        //        return shoppingCart.Product.Price;
        //    }
        //    else
        //    {
        //        if (shoppingCart.Quantity <= 100)
        //        {
        //            return shoppingCart.Product.Price;
        //        }
        //        else
        //        {
        //            return shoppingCart.Product.Price;
        //        }
        //    }
        //}
    }
}
