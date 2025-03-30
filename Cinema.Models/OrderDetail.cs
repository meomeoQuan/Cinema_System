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

        public int ? OrderID { get; set; } // Foreign key

        [NotMapped] // Không lưu vào database
        public string TempId { get; set; } // Dùng cho session

        public string UserId { get; set; } // Nullable cho khách

        [NotMapped]
        public DateTime AddedTime { get; set; } = DateTime.Now; // Dùng cho timeout session

        [Required]
        [Column("ProductId")] // Đảm bảo tên cột khớp với database
        public int ProductId { get; set; }

        [Required]
        [Column("ProductName")]
        public string ProductName { get; set; }

        //[Required]
        //public int UserId { get; set; }

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
        public string Showtime { get; set; }

        [Required]
        public int RoomId { get; set; } // Added RoomId

        [Required]
        public string RoomName { get; set; } // Added RoomName


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

        public double TotalPrice
        {
            get { return Price * Quantity; }
            set { } 
        }

        // Navigation properties
        [ForeignKey("OrderID")]
        public virtual OrderTable Order { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
