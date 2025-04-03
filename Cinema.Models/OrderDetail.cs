using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cinema.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace Cinema.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailID { get; set; }

        public int? OrderID { get; set; }


        [NotMapped] // Không lưu vào database
        public string? TempId { get; set; } // Dùng cho session

        //public string UserId { get; set; } // Nullable cho khách


        [NotMapped]
        public DateTime AddedTime { get; set; } = DateTime.Now; // Dùng cho timeout session

        [Column("ProductID")] // Đảm bảo tên cột khớp với database
        public int? ProductID { get; set; }     // Cho phép ProductID nullable để hỗ trợ đơn hàng chỉ có vé xem phim

        public int? ShowtimeSeatID { get; set; } // Nullable if the order is for products only


        //[Required]
        //[Column("ProductName")]
        //public string ProductName { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.00, 999999.99, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; } // Price per unit


        // List of selected tickets (multiple seats possible)

        //[NotMapped]
        //public List<TicketSelectionVM> Tickets { get; set; } = new List<TicketSelectionVM>();

        // List of selected food items (multiple items possible)
        [NotMapped]
        public List<FoodSelectionVM> FoodItems { get; set; } = new List<FoodSelectionVM>();

        public double TotalPrice
        {
            get { return Price * Quantity; }
            set { }
        }

        // remove this if error occured
        //[ForeignKey("UserID")]
        //[ValidateNever]
        //public virtual ApplicationUser User { get; set; }

        //

        [ForeignKey("OrderID")]
        [ValidateNever]
        [InverseProperty("OrderDetails")]
        public virtual OrderTable? Order { get; set; }

        [ForeignKey("ProductID")]
        [ValidateNever]
        public virtual Product? Product { get; set; }


        [ForeignKey("ShowtimeSeatID")]
        [ValidateNever]
        public virtual ShowtimeSeat? ShowtimeSeat { get; set; }
    }
}
