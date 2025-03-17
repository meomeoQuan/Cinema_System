using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models
{
    public class ShoppingCart
    {
        [Key]
        public int ShoppingCartID { get; set; }


        public int? ProductID { get; set; } // Nullable if the order is for tickets only

        public int? ShowtimeSeatID { get; set; } // Nullable if the order is for products only

        
        public int Quantity { get; set; } = 1;


        [NotMapped]
        public double Price { get; set; }


        public string? UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }
      

        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }

        [ForeignKey("ShowtimeSeatID")]
        public virtual ShowtimeSeat? ShowtimeSeat { get; set; }
    }
}

