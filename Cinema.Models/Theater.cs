using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace Cinema.Models
{
    public class Theater
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CinemaID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of rooms must be at least 1.")]
        public int NumberOfRooms { get; set; } = 1;

        [Required]
        [EnumDataType(typeof(CinemaStatus))]
        public CinemaStatus Status { get; set; } = CinemaStatus.Open;

        [Required]
        public TimeSpan OpeningTime { get; set; }

        [Required]
        public TimeSpan ClosingTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CinemaCity { get; set; }
        public string? AdminID { get; set; }

        [ForeignKey("AdminID")]
        [ValidateNever]
        [InverseProperty("Theaters")]
        public virtual ApplicationUser Admin { get; set; }

        //[JsonIgnore]
        [InverseProperty("Theater")]
        [ValidateNever]
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

        //[InverseProperty("Theater")]
        //public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

    }

    public enum CinemaStatus
    {
        Open,
        Closed
    }
}

