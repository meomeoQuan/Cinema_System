using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
        public string? Synopsis { get; set; }

        [Url]
        public string? TrailerLink { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? AgeLimit { get; set; }
        public bool IsUpcomingMovie { get; set; }
        //public string Actor { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [InverseProperty("Movie")]
        public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
        [ValidateNever]
        public string? MovieImage { get; set; } // validate never as it does not treat as normal input property

    }
    enum MovieStatus
    {
        Upcoming, NowShowing, Ended
    }
}