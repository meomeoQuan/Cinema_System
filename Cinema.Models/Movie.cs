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
        public int MovieID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Genre { get; set; }

        //public string Director { get; set; }
        public string Description { get; set; }
    
        public string TrailerLink { get; set; }
        public string Duration { get; set; }
        public string ReleaseDate { get; set; }
        //public string Actor { get; set; }

        public DateTime ? CreatedAt { get; set; }  // Auto-assign date when added
        public DateTime ? UpdatedAt { get; set; } 

        [ValidateNever]
        public string MovieImage { get; set; } // validate never as it does not treat as normal input property



    }
}
