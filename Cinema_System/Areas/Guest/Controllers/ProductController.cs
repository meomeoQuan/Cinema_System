using Cinema.DataAccess.Data;
using Cinema.Models;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Cinema_System;
using Cinema.DataAccess.Repository.IRepository;

namespace Cinema_System.Areas.Guest.Controllers
{
    [Area("Guest")]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _product;

        //public ProductController(ApplicationDbContext context)
        //{
        //    _product = context;
        //}

        private readonly IProductRepository _productRepo;

        public ProductController(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }
        [HttpGet]
        public IActionResult Product()
        {
            var viewproduct = new ProductVM
            {
                //Snacks = _product.Products.Where(p => p.ProductType == ProductType.Snack).Take(4).ToList(),
                //Drinks = _product.Products.Where(p => p.ProductType == ProductType.Drink).Take(4).ToList(),
                //Combos = _product.Products.Where(p => p.ProductType == ProductType.Combo).Take(3).ToList(),
                //Gifts = _product.Products.Where(p => p.ProductType == ProductType.Gift).Take(2).ToList()
                Snacks = _productRepo.GetProductsByType(ProductType.Snack),
                Drinks = _productRepo.GetProductsByType(ProductType.Drink),
                Combos = _productRepo.GetProductsByType(ProductType.Combo),
                Gifts = _productRepo.GetProductsByType(ProductType.Gift)
            };

            return View("Product", viewproduct);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Xử lý thêm vào giỏ hàng
            return RedirectToAction("Index");
        }
    }
}
