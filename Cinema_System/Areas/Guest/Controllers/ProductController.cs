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
            try
            {
                // 1. Check product availability
                var product = await _productRepo.GetAsync(p => p.ProductID == productId);
                if (product == null || product.Quantity < quantity)
                {
                    TempData["Error"] = "Product is not available or not in sufficient quantity";
                    return RedirectToAction("Product");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isStaff = User.IsInRole("Staff");

                // 2. Handle logged-in users (both regular users and staff)
                if (userId != null)
                {
                    // 2.1 Find or create Order
                    var order = await _unitOfWork.OrderTable
                        .GetAsync(o => o.UserID == userId && o.Status == OrderStatus.Pending);

                    if (order == null)
                    {
                        order = new OrderTable
                        {
                            UserID = userId,
                            Status = OrderStatus.Pending,
                            CreatedAt = DateTime.Now
                        };
                        _unitOfWork.OrderTable.Add(order);
                        await _unitOfWork.SaveAsync();
                    }

                    // 2.2 Check if product already in cart
                    var cartItem = await _unitOfWork.OrderDetail
                        .GetAsync(od => od.ProductID == productId && od.OrderID == order.OrderID);

                    if (cartItem != null)
                    {
                        cartItem.Quantity += quantity;
                        cartItem.TotalPrice = cartItem.Price * cartItem.Quantity;
                    }
                    else
                    {
                        _unitOfWork.OrderDetail.Add(new OrderDetail
                        {
                            OrderID = order.OrderID,
                            ProductID = product.ProductID,
                            Price = product.Price,
                            Quantity = quantity,
                            TotalPrice = product.Price * quantity
                        });
                    }
                    await _unitOfWork.SaveAsync();
                }
                // 3. Handle guest users (session cart with 5-minute timeout)
                else
                {
                    var cartItems = GetSessionCart();
                    var existingItem = cartItems.FirstOrDefault(i => i.ProductID == productId);

                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                        existingItem.TotalPrice = existingItem.Price * existingItem.Quantity;
                        existingItem.AddedTime = DateTime.Now; // Reset the timer when updated
                    }
                    else
                    {
                        cartItems.Add(new OrderDetail
                        {
                            TempId = Guid.NewGuid().ToString(),
                            ProductID = productId,
                            Price = product.Price,
                            Quantity = quantity,
                            TotalPrice = product.Price * quantity,
                            AddedTime = DateTime.Now,
                            Product = new Product // Store minimal product info
                            {
                                ProductID = product.ProductID,
                                Name = product.Name,
                                ProductImage = product.ProductImage,
                                Description = product.Description
                            }
                        });
                    }
                    SaveSessionCart(cartItems);
                }

                return RedirectToAction("Cart");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "There was an error while adding to cart";
                return RedirectToAction("Product");
            }
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

            // For all logged-in users (both regular and staff)
            if (userId != null)
            {
                var order = await _unitOfWork.OrderTable
                    .GetAsync(o => o.UserID == userId && o.Status == OrderStatus.Pending,
                                         includeProperties: "OrderDetails.Product");

                viewModel.DatabaseItems = order?.OrderDetails?.ToList() ?? new List<OrderDetail>();
            }
            // For guest users
            else
            {
                viewModel.SessionItems = GetSessionCart();

                // Load product info for session items
                var productIds = viewModel.SessionItems.Select(i => i.ProductID).Distinct().ToList();
                var products = await _productRepo.GetAllAsync(p => productIds.Contains(p.ProductID));

                foreach (var item in viewModel.SessionItems)
                {
                    item.Product = products.FirstOrDefault(p => p.ProductID == item.ProductID);
                }
            }

            // Calculate totals
            viewModel.Subtotal = (viewModel.DatabaseItems?.Sum(i => i.TotalPrice) ?? 0)
                               + (viewModel.SessionItems?.Sum(i => i.TotalPrice) ?? 0);
            viewModel.Total = viewModel.Subtotal - viewModel.Discount;
            viewModel.Message = TempData["CartMessage"]?.ToString();

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Product(string searchString, ProductType? productType)
        {
            var products = _productRepo.GetAll().AsQueryable();

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
