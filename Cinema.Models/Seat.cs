using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Seat
    {
        [Key]
        public int SeatID { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = "Row must be a single character (A-Z).")]
        public string Row { get; set; } = string.Empty; // Example: 'A', 'B', 'C'

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Column number must be at least 1.")]
        public int ColumnNumber { get; set; }

        [Required]
        public int RoomID { get; set; } // Foreign key

        [Required]
        public Boolean Status { get; set; } = true;

        // Navigation property
        [ForeignKey("RoomID")]
        public virtual Room Room { get; set; }
    }
}
