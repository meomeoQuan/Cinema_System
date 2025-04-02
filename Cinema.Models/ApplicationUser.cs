
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        [NotMapped]
        public string Role { get; set; }

        public string? UserImage { get; set; }
        public int Points { get; set; } = 0;
        [InverseProperty("Admin")]
        public virtual ICollection<Theater> Theaters { get; set; } = new List<Theater>();

        [InverseProperty("User")]
        public virtual ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
    }
}