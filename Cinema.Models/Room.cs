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
        public string RoomNumber { get; set; } = string.Empty;
        //public string RoomName { get; set; }


        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }
        //public int RoomCapacity { get; set; }

        [Required]
        [EnumDataType(typeof(RoomStatus))]
        public RoomStatus Status { get; set; } = RoomStatus.Available;
        //public int? RoomStatus { get; set; }


        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public int CinemaID { get; set; }
        [ForeignKey("CinemaID")]
        [ValidateNever]
        public Theater Cinema { get; set; }

        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
        // Navigation property: One Room has many ShowTimes
        public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();


    }
    public enum RoomStatus
    {
        Available,
        Maintenance
    }
}


