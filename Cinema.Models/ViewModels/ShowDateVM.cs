using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
    public  class ShowDateVM
    {
        public string  ShowDate { get; set; }
        public List<CityVM> Cities { get; set; }
    }
}
