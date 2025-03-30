using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
    public class ProductVM
    {
        public IEnumerable<Product> Snacks { get; set; }
        public IEnumerable<Product> Drinks { get; set; }
        public IEnumerable<Product> Combos { get; set; }
        public IEnumerable<Product> Gifts { get; set; }
        public string SearchString { get; set; }
        public ProductType? SelectedProductType { get; set; }
        public IEnumerable<ProductType> ProductTypes => Enum.GetValues<ProductType>();

    }
}
