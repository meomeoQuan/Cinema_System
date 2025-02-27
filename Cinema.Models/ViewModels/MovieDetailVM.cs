using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Models.ViewModels
{
    public class MovieDetailVM
    {

        public Movie Movie { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CinemaListItem { get; set; } // Dropdown for Cinemas

        public Theater Cinema { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> DateListItem { get; set; } // Dropdown for Dates

        public  ShowTime ShowTime { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> ShowTimeListItem  { get; set; } // Dropdown for Showtimes

      
        public IEnumerable<OrderDetail> OrderDetails { get; set; } // Shopping cart
    }
}
