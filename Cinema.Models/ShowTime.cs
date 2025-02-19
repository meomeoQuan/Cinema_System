using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cinema.Models
{
    public class ShowTime
    {
        [Key]
        public int ShowTimeID { get; set; }

        [Required]
        public string ShowDates { get; set; }
        [Required]
        public string ShowTimes { get; set; }


        public int CinemaID { get; set; }
        [ForeignKey(nameof(CinemaID))]
        [ValidateNever]
        public Cinema Cinema{ get; set; }


        public int RoomID { get; set; }
        [ForeignKey(nameof(RoomID))]
        [ValidateNever]
        public Room Room { get; set; }


        public int MovieID { get; set; }
        [ForeignKey(nameof(MovieID))]
        [ValidateNever]
        public Movie Movie { get; set; }

    }
}
