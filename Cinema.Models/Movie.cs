using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cinema.Models
{
    public class Movie
    {
        [Key]
        public int movieID { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string genre { get; set; }

        //public string Director { get; set; }
        public string description { get; set; }
    
        public string trailerLink { get; set; }
        public string duration { get; set; }
        public string releaseDate { get; set; }

        public string ageLimit { get; set; }
        //public string Actor { get; set; }

        public DateTime ? createdAt { get; set; }  // Auto-assign date when added
        public DateTime ? updatedAt { get; set; } 

        [ValidateNever]
        public string movieImage { get; set; } // validate never as it does not treat as normal input property



    }
}
