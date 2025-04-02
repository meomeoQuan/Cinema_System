using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;



namespace Cinema.Models
{
    public class OrderTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }



        [Required]
        public string UserID { get; set; } // IdentityUser uses string as primary key

        [Required]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public int? CouponID { get; set; } // Nullable (if no coupon is used)

        [Required]

        [Range(0.00, 999999.99, ErrorMessage = "Total amount must be a positive value.")]
        public double TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


        [ForeignKey("UserID")]
        [ValidateNever]
        //[InverseProperty("OrderTables")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("CouponID")]
        [ValidateNever]
        [InverseProperty("OrderTables")]
        public virtual Coupon? Coupon { get; set; }
        
        //public string ? Email { get; set; }
        //[StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 digits.")]
        //[RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be numeric and 10 digits long.")]
        //public string ? Phonenumber { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled
    }
}


