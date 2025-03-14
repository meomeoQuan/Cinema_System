using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cinema.Models.ViewModels;
using Newtonsoft.Json;

namespace Cinema.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; } // Foreign key

        [Required]
        public string ? UserId { get; set; }

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
        public int RoomId { get; set; }

        [Required]
        public string RoomName { get; set; }

        [Required]
        public string Showtime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Range(0.00, 999999.99, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; } // Price per unit

        // 🟢 **New properties for JSON serialization**
        [NotMapped]
        public string? TicketsJson { get; set; }

        [NotMapped]
        public string ? SelectedSeatsJson { get; set; }

        [NotMapped]
        public string? ItemsJson { get; set; }

        // 🟢 **Mapped lists**
        [NotMapped]
        public List<TicketSelectionVM> Tickets
        {
            get => string.IsNullOrEmpty(TicketsJson) ? new List<TicketSelectionVM>()
                : JsonConvert.DeserializeObject<List<TicketSelectionVM>>(TicketsJson);

            set => TicketsJson = JsonConvert.SerializeObject(value);
        }

        [NotMapped]
        public List<ShowtimeSeat> SelectedSeats
        {
            get => string.IsNullOrEmpty(SelectedSeatsJson) ? new List<ShowtimeSeat>()
                : JsonConvert.DeserializeObject<List<ShowtimeSeat>>(SelectedSeatsJson);

            set => SelectedSeatsJson = JsonConvert.SerializeObject(value);
        }

        [NotMapped]
        public List<FoodSelectionVM> FoodItems
        {
            get => string.IsNullOrEmpty(ItemsJson) ? new List<FoodSelectionVM>()
                : JsonConvert.DeserializeObject<List<FoodSelectionVM>>(ItemsJson);
            set => ItemsJson = JsonConvert.SerializeObject(value);
        }

        [NotMapped]
        public double TotalPrice { get; set; } = 0; // Subtotal

        // Navigation properties
        [ForeignKey("OrderID")]
        public virtual OrderTable Order { get; set; }
    }
}
