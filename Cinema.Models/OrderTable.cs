using System;
using System.Collections.Generic;
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

        // New Fields for Movie & Cinema Information
        [Required]
        public int MovieID { get; set; }

        [Required]
        public string MovieName { get; set; }

        [Required]
        public string ShowDate { get; set; } // e.g., "2025-03-15"

        [Required]
        public string City { get; set; }

        [Required]
        public string Cinema { get; set; }

        [Required]
        public string Showtime { get; set; } // e.g., "18:30"

        [Required]
        public int RoomID { get; set; }

        [Required]
        public string RoomName { get; set; }

        // 🟢 **Fix: Now it correctly calculates the order total**
        [Required]
        [Range(0.00, 999999.99, ErrorMessage = "Total amount must be a positive value.")]
        public double TotalAmount
        {
            get
            {
                return OrderDetails?.Sum(od => od.TotalPrice) ?? 0;
            }
        }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

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
