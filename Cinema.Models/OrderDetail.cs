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
        [Required]
            public int? ShowtimeSeatID { get; set; } // Nullable if the order is for products only

            
           
            public int Quantity { get; set; } = 1;


      
            public double Price { get; set; }


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
