using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
    public class CartVM
    {
        public List<OrderDetail> DatabaseItems { get; set; } = new List<OrderDetail>();
        public List<OrderDetail> SessionItems { get; set; } = new List<OrderDetail>();
        public double Subtotal { get; set; }
        public double Discount { get; set; }
        public double Total { get; set; }
        public string CouponCode { get; set; }
        public string Message { get; set; }
    }
}
