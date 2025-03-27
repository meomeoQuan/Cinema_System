using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Newtonsoft.Json;

namespace Cinema.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomID { get; set; }
        [Required]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
        public int Capacity { get; set; }

        [Required]
        [EnumDataType(typeof(RoomStatus))]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int CinemaID { get; set; }

        [ForeignKey("CinemaID")]
        //[InverseProperty("Rooms")]
        [ValidateNever]
        [JsonIgnore]
        public virtual Theater Theater { get; set; }
        [InverseProperty("Room")]
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
        [InverseProperty("Room")]
        public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();


    }
    public enum RoomStatus
    {
        Available,
        Maintenance
    }
}


