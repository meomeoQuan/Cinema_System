using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cinema.Models.ViewModels;

namespace Cinema.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; } // Foreign key

        //[Required]
        //public string UserId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public string MovieName { get; set; }

        [Required]
        public string Date { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Cinema { get; set; }

        [Required]
        public int RoomId { get; set; } // Added RoomId

        [Required]
        public string RoomName { get; set; } // Added RoomName

        [Required]
        public string Showtime { get; set; }

        // 🟢 **Added fields**
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.00, 999999.99, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; } // Price per unit

        // List of selected tickets (multiple seats possible)
        [NotMapped]
        public List<TicketSelectionVM> Tickets { get; set; } = new List<TicketSelectionVM>();

        // List of selected food items (multiple items possible)
        [NotMapped]
        public List<FoodSelectionVM> FoodItems { get; set; } = new List<FoodSelectionVM>();

        // 🟢 **Now, TotalPrice makes sense**
        [NotMapped]
        public double TotalPrice { get; set; } = 0;// **Subtotal for this item**

        // Navigation properties
        [ForeignKey("OrderID")]
        public virtual OrderTable Order { get; set; }
    }
}
