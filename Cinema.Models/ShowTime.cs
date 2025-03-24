﻿using System;
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
        //public string ShowDates { get; set; }




        public int RoomID { get; set; }
        [ForeignKey(nameof(RoomID))]
        [ValidateNever]
        public Room ? Room { get; set; }


        public int MovieID { get; set; }
        [ForeignKey(nameof(MovieID))]
        [ValidateNever]
        public Movie? Movie { get; set; }

        public virtual ICollection<ShowtimeSeat> ShowtimeSeats { get; set; } = new List<ShowtimeSeat>();

        [NotMapped]
        public int AvailableTicketQuantity => ShowtimeSeats.Count(s => s.Status == ShowtimeSeatStatus.Available);

    }
}
