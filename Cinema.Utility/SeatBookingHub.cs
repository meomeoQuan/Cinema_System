using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cinema.Utility
{
    public class SeatBookingHub : Hub
    {
        private static readonly Dictionary<string, string> BookedSeats = new Dictionary<string, string>(); // Key: seatId, Value: user

        public async Task BookSeat(string seatId, string user)
        {
            if (!BookedSeats.ContainsKey(seatId)) // If seat is free, allow booking
            {
                BookedSeats[seatId] = user;
                await Clients.All.SendAsync("SeatBooked", seatId, user);
            }
            else
            {
                await Clients.Caller.SendAsync("SeatBookingFailed", seatId, "Seat already booked");
            }
        }

        public async Task ReleaseSeat(string seatId, string user)
        {
            if (BookedSeats.ContainsKey(seatId) && BookedSeats[seatId] == user)
            {
                BookedSeats.Remove(seatId);
                await Clients.All.SendAsync("SeatReleased", seatId);
            }
            else
            {
                await Clients.Caller.SendAsync("SeatReleaseFailed", seatId, "Seat is not booked by you");
            }
        }

        public async Task<List<string>> GetBookedSeats()
        {
            return await Task.FromResult(new List<string>(BookedSeats.Keys));
        }
    }
}
