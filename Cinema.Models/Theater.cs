using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cinema.Models
{
    public class Theater
    {
        [Key]
        public int CinemaID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        //public string CinemaName { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;
        //public string CinemaAddress { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of rooms must be at least 1.")]
        public int NumberOfRooms { get; set; } = 1;

        [Required]
        [EnumDataType(typeof(CinemaStatus))]
        public CinemaStatus Status { get; set; } = CinemaStatus.Open;

        [Required]
        //public TimeSpan OpeningTime { get; set; }
        public string OpeningTime { get; set; }

        [Required]
        //public TimeSpan ClosingTime { get; set; }
        public string ClosingTime { get; set; }
        public DateTime ? CreatedAt { get; set; }
        public DateTime ? UpdatedAt { get; set; }

        public string? CinemaCity { get; set; }
        public string ? AdminID { get; set; } // Foreign key

        // Navigation property
        [ForeignKey("AdminID")]
        [ValidateNever]
        public virtual ApplicationUser Admin { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }

    }

    public enum CinemaStatus
    {
        Open,
        Closed
    }
}

