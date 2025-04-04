using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cinema.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = SD.Role_Staff)]
    public class ProductStaffController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductStaffController(IProductRepository productRepo, IWebHostEnvironment hostEnvironment)
        {
            _productRepo = productRepo;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var products = _productRepo.GetAll();
            return View(products);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productRepo.GetFirstOrDefault(p => p.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //GET: CREATE
        public IActionResult Create()
        {
            return View();
        }


        //POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, "css", "images");
                    var extension = Path.GetExtension(file.FileName);

                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    product.ProductImage = "/css/images/" + fileName + extension;
                }

                _productRepo.Add(product);
                _productRepo.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }


        //GET: Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productRepo.GetFirstOrDefault(p => p.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        //POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Product product, IFormFile? file)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, "css", "images");
                    var extension = Path.GetExtension(file.FileName);

                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }

                    if (!string.IsNullOrEmpty(product.ProductImage))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, product.ProductImage.TrimStart('/').Replace("/", "\\"));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    product.ProductImage = "/css/images/" + fileName + extension;
                }

                _productRepo.Update(product);
                _productRepo.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public IActionResult GetProductsByType(ProductType type)
        {
            var products = _productRepo.GetProductsByType(type);
            return View("Index", products);
        }
    }
}


