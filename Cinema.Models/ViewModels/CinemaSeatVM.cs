using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
    public class CinemaSeatVM
    {
        public CinemaSeatVM()
        {
            Status = ShowtimeSeatStatus.Available;
        }
        public int SeatId { get; set; }
        public string SeatName { get; set; }  // Seat Position (e.g., A1, B3)
        public string Row { get; set; }
        public TicketType SeatType { get; set; }
        public double Price { get; set; } // Set price dynamically based on type
        public ShowtimeSeatStatus Status { get; set; }  // Available/Booked


    }
}
