using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Models
{
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CouponID { get; set; }
        [Required]
        [StringLength(50)]
        //[Index(IsUnique = true)] // Ensures uniqueness in EF6 (not needed in EF Core)
        public string Code { get; set; } = string.Empty;
        [Required]
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public double DiscountPercentage { get; set; }
        [Required]
        [Range(0, 1000, ErrorMessage = "Max discount amount must be between 0 and 1000.")]
        public double? UsageLimit { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Used count cannot be negative.")]
        public int UsedCount { get; set; } = 0; // Default value to match SQL
        public DateTime? ExpireDate { get; set; } // Renamed from ExpireDay for clarity

        [InverseProperty("Coupon")]
        public virtual ICollection<OrderTable> OrderTables { get; set; } = new List<OrderTable>();

        [InverseProperty("Coupon")]
        public virtual ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
        [ValidateNever]
        public string? CouponImage { get; set; }
    }
}