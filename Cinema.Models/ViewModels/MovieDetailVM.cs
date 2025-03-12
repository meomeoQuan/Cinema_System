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
        public List<ShowDateVM> ShowDates { get; set; }

        // Shopping Cart
        public List<OrderDetail> OrderDetails { get; set; } // từ đây có thể lấy ra thông tin user id

        // trang này sẽ dc làm như là 1 cái shopping cart 
    }
}
