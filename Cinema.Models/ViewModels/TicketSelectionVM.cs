using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
    public class TicketSelectionVM
    {
        public int SeatId { get; set; }   // Unique seat ID
        public string SeatNumber { get; set; } // e.g., A1, B3
        public TicketType SeatType { get; set; }
        public double Price { get; set; }
    }
}
