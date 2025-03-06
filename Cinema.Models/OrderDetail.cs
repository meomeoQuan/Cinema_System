using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class OrderDetail // shopping cart
    {
        [Key]
        public int OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; } // Foreign key

        public int? ProductID { get; set; } // Nullable if the order is for tickets only

        public int? ShowtimeSeatID { get; set; } // Nullable if the order is for products only

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        [Required]

        [Range(0.00, 999999.99, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; }

        // Navigation properties
        [ForeignKey("OrderID")]
        public virtual OrderTable Order { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }

        [ForeignKey("ShowtimeSeatID")]
        public virtual ShowtimeSeat? ShowtimeSeat { get; set; }
    }
}
