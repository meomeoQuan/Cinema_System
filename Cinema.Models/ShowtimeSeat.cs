using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class ShowtimeSeat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShowtimeSeatID { get; set; }

        [Required]
        public int ShowtimeID { get; set; } // Foreign key

        [Required]
        public int SeatID { get; set; } // Foreign key

        [Required]
        
        [Range(0.00, 9999.99, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; }  // Default price

        [Required]
        [EnumDataType(typeof(ShowtimeSeatStatus))]
        public ShowtimeSeatStatus Status { get; set; } = ShowtimeSeatStatus.Available;

        // Navigation properties
        [ForeignKey("ShowtimeID")]
        public virtual ShowTime Showtime { get; set; }

        [ForeignKey("SeatID")]
        public virtual Seat Seat { get; set; }
    }

    public enum ShowtimeSeatStatus
    {
        Available,
        Maintenance,
        Booked
    }
}
