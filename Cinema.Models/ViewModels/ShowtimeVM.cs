using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Models.ViewModels
{
    public class ShowtimeVM
    {
        public string Date { get; set; } = string.Empty;
        public string CinemaName { get; set; } = string.Empty;
        public int CinemaId { get; set; }
        public string CinemaAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Showtime { get; set; } = string.Empty;

        public List<SeatVM> SeatList { get; set; } = new();
    }
}
