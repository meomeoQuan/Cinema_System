using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
      public class CinemaScheduleVM
    {
        public int CinemaID { get; set; }
        public string CinemaName { get; set; }

        // List of Available Dates & Showtimes
        public List<ShowTimeInfoVM> AvailableDates { get; set; } = new List<ShowTimeInfoVM>();
    }
}
