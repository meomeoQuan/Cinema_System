using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovieID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Genre { get; set; }

        //public string Director { get; set; }
        //public string Description { get; set; }
        public string? Synopsis { get; set; }

        [Url]
        public string? TrailerLink { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int Duration { get; set; }
        //public string Duration { get; set; }
        //public string ReleaseDate { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string AgeLimit { get; set; }
        public bool IsUpcomingMovie { get; set; }
        //public string Actor { get; set; }
        //public bool IsUpcommingMovie { get; set; }
        public DateTime ? CreatedAt { get; set; }  // Auto-assign date when added
        public DateTime ? UpdatedAt { get; set; }
        public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
        [ValidateNever]
        public string MovieImage { get; set; } // validate never as it does not treat as normal input property



    }
}