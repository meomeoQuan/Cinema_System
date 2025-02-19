using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models
{
    public class Coupon
    {
        [Key] 
        public int CouponID { get; set; }
        [Required]
        public string CouponCode { get; set; }
        public string DiscountPercentage { get; set; }
        
        public int ? UsageLimit { get; set; }
        public int ? UsedCount { get; set; }

        public DateTime ? ExpireDay { get; set; }
    }
}
