using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Cinema.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;


[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CouponsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CouponsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var coupons = await _unitOfWork.Coupon.GetAllAsync();
        return View(coupons);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var coupons = await _unitOfWork.Coupon.GetAllAsync();
        var couponsList = coupons.Select(c => new
        {
            c.CouponID,
            c.Code,
            c.DiscountPercentage,
            UsageLimit = c.UsageLimit, // Now represents max discount amount
            c.UsedCount,
            c.ExpireDate
        }).ToList();

        return Json(new { data = couponsList });
    }

    [HttpPost]
    public async Task<IActionResult> Create(Coupon coupon)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Kiểm tra trùng mã
                var existingCoupon = await _unitOfWork.Coupon.GetAsync(c =>
                     c.Code.ToLower() == coupon.Code.ToLower());

                if (existingCoupon != null)
                {
                    return Json(new { success = false, message = "Coupon code already exists" });
                }

                // Đảm bảo UsedCount = 0 khi tạo mới
                coupon.UsedCount = 0;

                await _unitOfWork.Coupon.AddAsync(coupon);
                await _unitOfWork.SaveAsync();

                return Json(new { success = true, message = "Coupon created successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error creating coupon: {ex.Message}" });
            }
        }
        return Json(new { success = false, message = "Invalid coupon data." });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCouponField(int id, string field, string value)
    {
        var coupon = await _unitOfWork.Coupon.GetAsync(c => c.CouponID == id);
        if (coupon == null)
        {
            return Json(new { success = false, message = "Coupon not found." });
        }

        try
        {
            switch (field)
            {
                case "Code":
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return Json(new { success = false, message = "Code cannot be empty." });
                    }
                    // Kiểm tra trùng mã (phân biệt hoa thường)
                    var existingCoupon = await _unitOfWork.Coupon.GetAsync(c =>
                    c.Code.ToLower() == value.ToLower() &&
                    c.CouponID != id);
                    if (existingCoupon != null)
                    {
                        return Json(new { success = false, message = "Coupon code already exists." });
                    }
                    coupon.Code = value;
                    break;
                case "DiscountPercentage":
                    if (!double.TryParse(value, out double discountPercentage) || discountPercentage < 0 || discountPercentage > 100)
                    {
                        return Json(new { success = false, message = "Discount percentage must be between 0 and 100." });
                    }
                    coupon.DiscountPercentage = discountPercentage;
                    break;
                case "UsageLimit":
                    if (!double.TryParse(value, out double usageLimit) || usageLimit <= 0)
                    {
                        return Json(new { success = false, message = "Max discount amount must be greater than 0." });
                    }
                    coupon.UsageLimit = usageLimit;
                    break;
                case "UsedCount":
                    if (!int.TryParse(value, out int usedCount) || usedCount < 0)
                    {
                        return Json(new { success = false, message = "Used count cannot be negative." });
                    }
                    coupon.UsedCount = usedCount;
                    break;
                default:
                    return Json(new { success = false, message = "Invalid field." });
            }

            _unitOfWork.Coupon.Update(coupon);
            await _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Coupon updated successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error updating coupon: {ex.Message}" });
        }
    }

    [HttpPost("Apply")]
    public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrEmpty(request.CouponCode))
            {
                return Json(new { success = false, message = "Invalid request data" });
            }

            // Tìm coupon (không phân biệt hoa thường)
            var coupon = await _unitOfWork.Coupon.GetAsync(c =>
                c.Code.ToLower() == request.CouponCode.ToLower());

            if (coupon == null)
            {
                return Json(new { success = false, message = "Coupon not found" });
            }

            // Kiểm tra hết hạn
            if (coupon.ExpireDate.HasValue && coupon.ExpireDate < DateTime.Now)
            {
                return Json(new { success = false, message = "Coupon has expired" });
            }

            // Tính toán số tiền giảm giá
            decimal discountAmount = request.OrderTotal * (decimal)(coupon.DiscountPercentage / 100);

            // Áp dụng giới hạn tối đa
            if (coupon.UsageLimit.HasValue && discountAmount > (decimal)coupon.UsageLimit.Value)
            {
                discountAmount = (decimal)coupon.UsageLimit.Value;
            }

            // Tăng số lần sử dụng
            coupon.UsedCount++;
            _unitOfWork.Coupon.Update(coupon);
            await _unitOfWork.SaveAsync();

            return Json(new
            {
                success = true,
                discountAmount,
                newTotal = request.OrderTotal - discountAmount,
                message = $"Coupon applied successfully. Discount: {discountAmount.ToString("C")}"
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = $"Error applying coupon: {ex.Message}"
            });
        }
    }

    public class ApplyCouponRequest
    {
        public string CouponCode { get; set; }
        public decimal OrderTotal { get; set; }
    }

    // API kiểm tra coupon
    [HttpGet("Validate/{code}")]
    public async Task<IActionResult> ValidateCoupon(string code)
    {
        // Tìm coupon (phân biệt hoa thường)
        var coupon = await _unitOfWork.Coupon.GetAsync(c =>
                     c.Code.ToLower() == code.ToLower());

        if (coupon == null)
        {
            return Json(new { isValid = false, message = "Coupon not found" });
        }

        // Kiểm tra hết hạn
        bool isExpired = coupon.ExpireDate.HasValue && coupon.ExpireDate < DateTime.Now;
        bool isValid = !isExpired;

        string statusMessage = isExpired ? "Coupon has expired" : "Valid coupon";

        return Json(new
        {
            isValid,
            message = statusMessage,
            coupon
        });
    }
}


//[Area("Admin")]
////[Authorize(Roles = SD.Role_Admin)]
//public class CouponsController : Controller
//{
//    private readonly IUnitOfWork _unitOfWork;

//    public CouponsController(IUnitOfWork unitOfWork)
//    {
//        _unitOfWork = unitOfWork;
//    }

//    public async Task<IActionResult> Index()
//    {
//        var coupons = await _unitOfWork.Coupon.GetAllAsync();
//        return View(coupons);
//    }

//    [HttpGet("GetAll")]
//    public async Task<IActionResult> GetAll()
//    {
//        var coupons = await _unitOfWork.Coupon.GetAllAsync();
//        var couponsList = coupons.Select(c => new
//        {
//            c.CouponID,
//            c.Code,
//            c.DiscountPercentage,
//            c.UsageLimit,
//            c.UsedCount,
//            c.ExpireDate,
//            Status = GetCouponStatus(c) // Thêm trạng thái coupon
//        }).ToList();

//        return Json(new { data = couponsList });
//    }

//    //trạng thái coupon
//    private string GetCouponStatus(Coupon coupon)
//    {
//        if (coupon.ExpireDate.HasValue && coupon.ExpireDate < DateTime.Now)
//            return "Expired";
//        if (coupon.UsedCount >= coupon.UsageLimit)
//            return "Used Up";
//        return "Active";
//    }

//    [HttpPost]
//    public async Task<IActionResult> Create(Coupon coupon)
//    {
//        if (ModelState.IsValid)
//        {
//            try
//            {
//                // Kiểm tra trùng mã
//                var existingCoupon = await _unitOfWork.Coupon.GetAsync(c =>
//                     c.Code.ToLower() == coupon.Code.ToLower());

//                if (existingCoupon != null)
//                {
//                    return Json(new { success = false, message = "Coupon code already exists" });
//                }

//                // Đảm bảo UsedCount = 0 khi tạo mới
//                coupon.UsedCount = 0;

//                await _unitOfWork.Coupon.AddAsync(coupon);
//                await _unitOfWork.SaveAsync();

//                return Json(new { success = true, message = "Coupon created successfully." });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = $"Error creating coupon: {ex.Message}" });
//            }
//        }
//        return Json(new { success = false, message = "Invalid coupon data." });
//    }

//    [HttpPost]
//    public async Task<IActionResult> UpdateCouponField(int id, string field, string value)
//    {
//        var coupon = await _unitOfWork.Coupon.GetAsync(c => c.CouponID == id);
//        if (coupon == null)
//        {
//            return Json(new { success = false, message = "Coupon not found." });
//        }

//        try
//        {
//            switch (field)
//            {
//                case "Code":
//                    if (string.IsNullOrWhiteSpace(value))
//                    {
//                        return Json(new { success = false, message = "Code cannot be empty." });
//                    }
//                    // Kiểm tra trùng mã (phân biệt hoa thường)
//                    var existingCoupon = await _unitOfWork.Coupon.GetAsync(c =>
//                    c.Code.ToLower() == value.ToLower() &&
//                    c.CouponID != id);
//                    if (existingCoupon != null)
//                    {
//                        return Json(new { success = false, message = "Coupon code already exists." });
//                    }
//                    coupon.Code = value;
//                    break;
//                case "DiscountPercentage":
//                    if (!double.TryParse(value, out double discountPercentage) || discountPercentage < 0 || discountPercentage > 100)
//                    {
//                        return Json(new { success = false, message = "Discount percentage must be between 0 and 100." });
//                    }
//                    coupon.DiscountPercentage = discountPercentage;
//                    break;
//                case "UsageLimit":
//                    if (!double.TryParse(value, out double usageLimit) || usageLimit < 0 || usageLimit > 1000)
//                    {
//                        return Json(new { success = false, message = "Usage limit must be between 0 and 1000." });
//                    }
//                    coupon.UsageLimit = usageLimit;
//                    break;
//                case "UsedCount":
//                    if (!int.TryParse(value, out int usedCount) || usedCount < 0)
//                    {
//                        return Json(new { success = false, message = "Used count cannot be negative." });
//                    }
//                    coupon.UsedCount = usedCount;
//                    break;
//                default:
//                    return Json(new { success = false, message = "Invalid field." });
//            }

//            _unitOfWork.Coupon.Update(coupon);
//            await _unitOfWork.SaveAsync();
//            return Json(new { success = true, message = "Coupon updated successfully." });
//        }
//        catch (Exception ex)
//        {
//            return Json(new { success = false, message = $"Error updating coupon: {ex.Message}" });
//        }
//    }

//    [HttpPost("Apply")]
//    public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponRequest request)
//    {
//        try
//        {
//            if (request == null || string.IsNullOrEmpty(request.CouponCode))
//            {
//                return Json(new { success = false, message = "Invalid request data" });
//            }

//            // Tìm coupon (không phân biệt hoa thường)
//            var coupon = await _unitOfWork.Coupon.GetAsync(c =>
//                c.Code.ToLower() == request.CouponCode.ToLower());

//            if (coupon == null)
//            {
//                return Json(new { success = false, message = "Coupon not found" });
//            }

//            // Kiểm tra hết hạn
//            if (coupon.ExpireDate.HasValue && coupon.ExpireDate < DateTime.Now)
//            {
//                return Json(new { success = false, message = "Coupon has expired" });
//            }

//            // Kiểm tra số lần sử dụng
//            if (coupon.UsedCount >= coupon.UsageLimit)
//            {
//                return Json(new { success = false, message = "Coupon usage limit reached" });
//            }

//            // Tính toán số tiền giảm giá
//            decimal discountAmount = request.OrderTotal * (decimal)(coupon.DiscountPercentage / 100);

//            // Cập nhật thông tin coupon
//            coupon.UsedCount++;
//            _unitOfWork.Coupon.Update(coupon);
//            await _unitOfWork.SaveAsync();

//            return Json(new
//            {
//                success = true,
//                discountAmount,
//                newTotal = request.OrderTotal - discountAmount,
//                message = $"Coupon applied successfully. Discount: {discountAmount.ToString("C")}",
//                remainingUses = coupon.UsageLimit - coupon.UsedCount
//            });
//        }
//        catch (Exception ex)
//        {
//            return Json(new
//            {
//                success = false,
//                message = $"Error applying coupon: {ex.Message}"
//            });
//        }
//    }

//    public class ApplyCouponRequest
//    {
//        public string CouponCode { get; set; }
//        public decimal OrderTotal { get; set; }
//    }

//    // API kiểm tra coupon
//    [HttpGet("Validate/{code}")]
//    public async Task<IActionResult> ValidateCoupon(string code)
//    {
//        // Tìm coupon (phân biệt hoa thường)
//        var coupon = await _unitOfWork.Coupon.GetAsync(c =>
//                     c.Code.ToLower() == code.ToLower());

//        if (coupon == null)
//        {
//            return Json(new { isValid = false, message = "Coupon not found" });
//        }

//        // Kiểm tra hết hạn
//        bool isExpired = coupon.ExpireDate.HasValue && coupon.ExpireDate < DateTime.Now;
//        bool isUsedUp = coupon.UsedCount >= coupon.UsageLimit;
//        bool isValid = !isExpired && !isUsedUp;

//        string statusMessage = isExpired ? "Coupon has expired" :
//                             (isUsedUp ? "Coupon usage limit reached" : "Valid coupon");

//        return Json(new
//        {
//            isValid,
//            message = statusMessage,
//            coupon
//        });
//    }
//}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Cinema.DataAccess.Data;
//using Cinema.DataAccess.Repository.IRepository;
//using Cinema.Models;
//using Cinema.Utility;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Cinema_System.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    //[Authorize(Roles = SD.Role_Admin)]
//    public class CouponsController : Controller
//    {
//        private readonly IUnitOfWork _unitOfWork;

//        public CouponsController(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var coupons = await _unitOfWork.Coupon.GetAllAsync();
//            return View(coupons);
//        }

//        [HttpGet("GetAll")]
//        public async Task<IActionResult> GetAll()
//        {
//            var coupons = await _unitOfWork.Coupon.GetAllAsync();
//            var couponsList = coupons.Select(c => new
//            {
//                c.CouponID,
//                c.Code,
//                c.DiscountPercentage,
//                c.UsageLimit,
//                c.UsedCount,
//                c.ExpireDate
//            }).ToList();

//            return Json(new { data = couponsList });
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(Coupon coupon)
//        {
//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    if (_unitOfWork.Coupon.Get(c => c.Code == coupon.Code) != null)
//                    {
//                        return Json(new { success = false, message = "Coupon code already exists." });
//                    }

//                    await _unitOfWork.Coupon.AddAsync(coupon);
//                    await _unitOfWork.SaveAsync();

//                    return Json(new { success = true, message = "Coupon created successfully." });
//                }
//                catch (Exception ex)
//                {
//                    return Json(new { success = false, message = $"Error creating coupon: {ex.Message}" });
//                }
//            }
//            return Json(new { success = false, message = "Invalid coupon data." });
//        }



//        [HttpPost]
//        public async Task<IActionResult> UpdateCouponField(int id, string field, string value)
//        {
//            var coupon = await _unitOfWork.Coupon.GetAsync(c => c.CouponID == id);
//            if (coupon == null)
//            {
//                return Json(new { success = false, message = "Coupon not found." });
//            }

//            try
//            {
//                switch (field)
//                {
//                    case "Code":
//                        if (string.IsNullOrWhiteSpace(value))
//                        {
//                            return Json(new { success = false, message = "Code cannot be empty." });
//                        }
//                        if (_unitOfWork.Coupon.Get(c => c.Code == value && c.CouponID != id) != null)
//                        {
//                            return Json(new { success = false, message = "Coupon code already exists." });
//                        }
//                        coupon.Code = value;
//                        break;
//                    case "DiscountPercentage":
//    if (!double.TryParse(value, out double discountPercentage) || discountPercentage < 0 || discountPercentage > 100)
//    {
//        return Json(new { success = false, message = "Discount percentage must be between 0 and 100." });
//    }
//    coupon.DiscountPercentage = discountPercentage;
//    break;
//case "UsageLimit":
//    if (!double.TryParse(value, out double usageLimit) || usageLimit < 0 || usageLimit > 1000)
//    {
//        return Json(new { success = false, message = "Usage limit must be between 0 and 1000." });
//    }
//    coupon.UsageLimit = usageLimit;
//    break;
//case "UsedCount":
//    if (!int.TryParse(value, out int usedCount) || usedCount < 0)
//    {
//        return Json(new { success = false, message = "Used count cannot be negative." });
//    }
//    coupon.UsedCount = usedCount;
//    break;
//default:
//    return Json(new { success = false, message = "Invalid field." });
//                }

//                _unitOfWork.Coupon.Update(coupon);
//                await _unitOfWork.SaveAsync();
//                return Json(new { success = true, message = "Coupon updated successfully." });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = $"Error updating coupon: {ex.Message}" });
//            }
//        }



//    }
//}
