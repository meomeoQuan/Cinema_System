using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinema.Models
{
    public class ShowTime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ShowTimeID { get; set; }

        [Required]
        public DateTime ShowDate { get; set; } // SQL `DATE` maps to `DateTime`
        public TimeSpan ShowTimes { get; set; }
        //public int CinemaID { get; set; }
        public int RoomID { get; set; }
        public int MovieID { get; set; }

        //[ForeignKey("CinemaId")]
        //[InverseProperty("Showtimes")]
        //public virtual Theater Theater { get; set; } = null!;
        //[ForeignKey("CinemaID")]
        //[InverseProperty("ShowTimes")]
        //[ValidateNever] 
        //public virtual Theater Theater { get; set; } = null!;


        [ForeignKey("MovieID")]
        [InverseProperty("ShowTimes")]
        [ValidateNever]
        public virtual Movie Movie { get; set; } = null!;

        [ForeignKey("RoomID")]
        [InverseProperty("ShowTimes")]
        [ValidateNever]
        public virtual Room Room { get; set; } = null!;

        [InverseProperty("Showtime")] // Ensure it matches the navigation property in `ShowtimeSeat`
        public virtual ICollection<ShowtimeSeat> ShowTimeSeats { get; set; } = new List<ShowtimeSeat>();


        //[NotMapped]
        //public int AvailableTicketQuantity => ShowtimeSeats.Count(s => s.Status == ShowtimeSeatStatus.Available);

    }
}
