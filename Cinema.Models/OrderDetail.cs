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

            [Required]
            public int OrderID { get; set; } // Foreign key
        
            public int? ProductID { get; set; } // Nullable if the order is for tickets only
        
            public int? ShowtimeSeatID { get; set; } // Nullable if the order is for products only


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


      
        


        public string? UserID { get; set; }
        [ForeignKey("UserID")]
        [ValidateNever]
        public virtual ApplicationUser User { get; set; }
        // Navigation properties
        [ForeignKey("OrderID")]
        [ValidateNever]
        public virtual OrderTable Order { get; set; }


            [ForeignKey("ProductID")]
        [ValidateNever]
            public virtual Product? Product { get; set; }   

            [ForeignKey("ShowtimeSeatID")]
        [ValidateNever]
        public virtual ShowtimeSeat? ShowtimeSeat { get; set; }
        }
    }
