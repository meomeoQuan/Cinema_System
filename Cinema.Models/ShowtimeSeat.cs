using Cinema.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cinema.Models
{
    public class ShowtimeSeat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShowtimeSeatID { get; set; }

        [Required]
        public int ShowtimeID { get; set; }

        [Required]
        public int SeatID { get; set; }

        [Required]
        [Range(0.00, 9999.99, ErrorMessage = "Price must be a positive value.")]
        public double Price { get; set; }  // Default price

        [Required]
        [EnumDataType(typeof(ShowtimeSeatStatus))]
        public ShowtimeSeatStatus Status { get; set; } = ShowtimeSeatStatus.Available;

        // Navigation properties
        [ForeignKey("ShowtimeID")]
        [InverseProperty("ShowTimeSeats")] 
        public virtual ShowTime Showtime { get; set; }


        [ForeignKey("SeatID")]
        [ValidateNever]
        [InverseProperty("ShowtimeSeats")] 
        public virtual Seat Seat { get; set; }
    }

    public enum ShowtimeSeatStatus
    {
        Available,
        Maintenance,
        Booked
    }
}
