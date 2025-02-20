using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models
{
    public class Cinema
    {
        [Key]
        public int CinemaID { get; set; }
        [Required]
        public string CinemaName { get; set; }
        [Required]
        public string CinemaAddress { get; set; }
        [Required]
        public int NumberOfCinemaRooms { get; set; }

        public int? CinemaStatus { get; set; }
        [Required]
        public string OpenningTime { get; set; }
        [Required]
        public string ClosingTime { get; set; }
        public DateTime ? CreatedAt { get; set; }
        public DateTime ? UpdatedAt { get; set; }

        //public int AdminID { get; set; }

    }
}
