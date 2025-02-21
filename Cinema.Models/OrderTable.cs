using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class OrderTable
    {
        [Key]
        public int OrderID { get; set; }

        [Required]
        public string UserID { get; set; } // IdentityUser uses string as primary key

        [Required]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public int? CouponID { get; set; } // Nullable (if no coupon is used)

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.00, 999999.99, ErrorMessage = "Total amount must be a positive value.")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("CouponID")]
        public virtual Coupon? Coupon { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled,
        Refunded
    }
}
