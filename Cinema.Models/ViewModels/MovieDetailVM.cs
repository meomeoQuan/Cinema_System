using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Models.ViewModels
{
    public class MovieDetailVM
    {
        public Movie Movie { get; set; }
        public OrderTable OrderTable { get; set; }
        // List to store multiple seats
        public List<ShoppingCart> ListCart { get; set; }
        public List<ShowtimeSeat> ShowtimeSeats { get; set; }

        //public ShoppingCart ShoppingCart { get; set; }
        public List<Product> products { get; set; }
    }
}
