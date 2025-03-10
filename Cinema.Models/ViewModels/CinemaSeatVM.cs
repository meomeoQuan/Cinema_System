using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
    public class CinemaSeatVM
    {
        public int SeatId { get; set; }
        public string SeatName { get; set; }
        public string Row { get; set; }  // <-- Add this property
        public List<string> listSeatGrid { get; set; }

    }
}
