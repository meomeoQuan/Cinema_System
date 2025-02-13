using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cinema.Models
{
    public class Product
    {
        [Key]
        public int productID { get; set; }

        [Required]
        public string nameProduct { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public string productType { get; set; }
        [Required]
        public double price { get; set; }

      

        [ValidateNever]
        public string ?productImage { get; set; }
    }

}

