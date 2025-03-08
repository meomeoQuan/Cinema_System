using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
    public class CityVM
    {
        public string CityName { get; set; }
        public List<TimeScheduleVM> Cinemas { get; set; }
    }
}
