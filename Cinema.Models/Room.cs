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
    public class Room
    {
        [Key]
        public int RoomID { get; set; }
        [Required]
        public string RoomName { get; set; }

        [Required]
        public int RoomCapacity { get; set; }

        public int? RoomStatus { get; set; }


        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public int CinemaID { get; set; }
        [ForeignKey("CinemaID")]
        [ValidateNever]
        public Cinema Cinema { get; set; } 

    }
}
