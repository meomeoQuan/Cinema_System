using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
      public class TimeScheduleVM
    {
        public int CinemaID { get; set; }
        public string CinemaName { get; set; }
        //------------------------------- there is two variables due to we just want to display these 2 
        //------------------------------- or if we take all define public Theater to accessible take all 

        public List<string> ShowTimes { get; set; }
        public List<CinemaSeatVM> Seats { get; set; }
    }
}
