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
        public List<string> ShowTimes { get; set; }  // Showtimes list
        // Key = Showtime (e.g., "18:30"), Value = List of available seats
        public Dictionary<string, List<CinemaSeatVM>> SeatsByShowtime { get; set; } // Map showtime to seat availability
    }
}
