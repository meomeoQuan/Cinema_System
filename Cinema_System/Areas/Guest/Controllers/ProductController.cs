using Cinema.DataAccess.Data;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Cinema_System;
using Cinema.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Cinema.DataAccess.Repository;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class ProductController : Controller
    {

        private readonly IProductRepository _productRepo;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IProductRepository productRepo, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        private List<OrderDetail> GetSessionCart()
        {
            var sessionCart = HttpContext.Session.GetString("Cart");
            if (sessionCart == null) return new List<OrderDetail>();

            var items = JsonConvert.DeserializeObject<List<OrderDetail>>(sessionCart);

            // Kiểm tra timeout 5 phút
            var now = DateTime.Now;
            var expiredItems = items.Where(i => (now - i.AddedTime).TotalMinutes > 5).ToList();

            if (expiredItems.Any())
            {
                items = items.Except(expiredItems).ToList();
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(items));
                TempData["CartMessage"] = "Your cart has expired due to inactivity";
            }

            return items;
        }

        private void SaveSessionCart(List<OrderDetail> items)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(items));
        }

        [HttpPost]

        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productRepo.GetAsync(p => p.ProductID == productId);
            if (product == null || product.Quantity < quantity)
            {
                TempData["Error"] = "Sản phẩm không có sẵn hoặc không đủ số lượng";
                return RedirectToAction("Product");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                // Đã đăng nhập - lưu vào database
                var cartItem = await _unitOfWork.OrderDetail
                    .GetAsync(od =>
                        od.ProductId == productId &&
                        od.OrderID == null);

                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                    cartItem.TotalPrice = cartItem.Price * cartItem.Quantity;
                }
                else
                {
                    //var defaultShowInfo = GetDefaultShowInfo(); // Phương thức giả định

                    _unitOfWork.OrderDetail.Add(new OrderDetail
                    {
                        OrderID = null,
                        ProductId = product.ProductID,
                        Price = product.Price,
                        Quantity = quantity,
                        TotalPrice = product.Price * quantity,
                        FoodItems = new List<FoodSelectionVM>()
                    });
                }
                await _unitOfWork.SaveAsync();
            }
            else
            {
                // Chưa đăng nhập - lưu vào session
                var cartItems = GetSessionCart();
                var existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.TotalPrice = existingItem.Price * existingItem.Quantity;
                }
                else
                {
                    cartItems.Add(new OrderDetail
                    {
                        TempId = Guid.NewGuid().ToString(),
                        ProductId = productId,
                        Price = product.Price,
                        Quantity = quantity,
                        TotalPrice = product.Price * quantity,
                        AddedTime = DateTime.Now
                    });
                }
                SaveSessionCart(cartItems);
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(string id, bool isSessionItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null && !isSessionItem)
            {
                // Xóa từ database
                if (int.TryParse(id, out int orderDetailId))
                {
                    var item = await _unitOfWork.OrderDetail.GetAsync(o => o.OrderDetailID == orderDetailId);
                    if (item != null)
                    {
                        _unitOfWork.OrderDetail.Remove(item);
                        await _unitOfWork.SaveAsync();
                    }
                }
            }
            else
            {
                // Xóa từ session
                var cartItems = GetSessionCart();
                var item = cartItems.FirstOrDefault(i => i.TempId == id);
                if (item != null)
                {
                    cartItems.Remove(item);
                    SaveSessionCart(cartItems);
                }
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(string id, int quantity, bool isSessionItem)
        {
            if (quantity < 1) return RedirectToAction("Cart");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null && !isSessionItem)
            {
                // Cập nhật database
                if (int.TryParse(id, out int orderDetailId))
                {
                    var item = await _unitOfWork.OrderDetail.GetAsync(o => o.OrderDetailID == orderDetailId);
                    if (item != null)
                    {
                        item.Quantity = quantity;
                        item.TotalPrice = item.Price * quantity;
                        await _unitOfWork.SaveAsync();
                    }
                }
            }
            else
            {
                // Cập nhật session
                var cartItems = GetSessionCart();
                var item = cartItems.FirstOrDefault(i => i.TempId == id);
                if (item != null)
                {
                    item.Quantity = quantity;
                    item.TotalPrice = item.Price * quantity;
                    SaveSessionCart(cartItems);
                }
            }

            return RedirectToAction("Cart");
        }

        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var viewModel = new CartVM();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var order = await _unitOfWork.OrderTable.GetAsync(o => o.UserID == userId && o.Status == OrderStatus.Pending);
                var orderDetails = await _unitOfWork.OrderDetail.GetAllAsync(o => o.OrderID == order.OrderID);

                viewModel.DatabaseItems = orderDetails.ToList();
            }
            else
            {
                // Lấy từ session cho guest
                viewModel.SessionItems = GetSessionCart();

                // Lấy thông tin Product cho session items
                var productIds = viewModel.SessionItems.Select(i => i.ProductId).ToList();
                var products = _productRepo.GetAll()
                    .Where(p => productIds.Contains(p.ProductID))
                    .ToList();

                foreach (var item in viewModel.SessionItems)
                {
                    item.Product = products.FirstOrDefault(p => p.ProductID == item.ProductId);
                }
            }

            // Tính tổng tiền
            var allItems = new List<OrderDetail>();
            allItems.AddRange(viewModel.SessionItems);
            allItems.AddRange(viewModel.DatabaseItems);

            viewModel.Subtotal = allItems.Sum(i => i.TotalPrice);
            viewModel.Total = viewModel.Subtotal - viewModel.Discount;
            viewModel.Message = TempData["CartMessage"]?.ToString();

            return View(viewModel);
        }

        //private ShowInfo GetDefaultShowInfo()
        //{
        //    return new ShowInfo
        //    {
        //        MovieId = 0,
        //        MovieName = string.Empty,
        //        Date = DateTime.Now.ToString("yyyy-MM-dd"),
        //        City = string.Empty,
        //        Cinema = string.Empty,
        //        Showtime = string.Empty,
        //        RoomId = -1,
        //        RoomName = string.Empty
        //    };
        //}

        //// Lớp hỗ trợ
        //public class ShowInfo
        //{
        //    public int MovieId { get; set; }
        //    public string MovieName { get; set; }
        //    public string Date { get; set; }
        //    public string City { get; set; }
        //    public string Cinema { get; set; }
        //    public string Showtime { get; set; }
        //    public int RoomId { get; set; }
        //    public string RoomName { get; set; }
        //}

        [HttpGet]
        public IActionResult Product(string searchString, ProductType? productType)
        {
            var products = _productRepo.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p =>
                    p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (productType.HasValue)
            {
                products = products.Where(p => p.ProductType == productType.Value);
            }

            var viewproduct = new ProductVM
            {
                Snacks = products.Where(p => p.ProductType == ProductType.Snack).ToList(),
                Drinks = products.Where(p => p.ProductType == ProductType.Drink).ToList(),
                Combos = products.Where(p => p.ProductType == ProductType.Combo).ToList(),
                Gifts = products.Where(p => p.ProductType == ProductType.Gift).ToList(),
                SearchString = searchString,
                SelectedProductType = productType
            };

            return View("Product", viewproduct);
        }
    }
}
