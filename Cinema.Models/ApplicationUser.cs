using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string fullName { get; set; }
        [NotMapped] 
        public string role { get; set; }

        public string ? userImage { get; set; }
    }
}
