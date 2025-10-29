using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading;
using Cinema_System.Areas.Guest.Controllers;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.Models;

namespace Cinema_System.Areas.Request
{
    public class CountdownHub : Hub
    {
        private static int countdownTime = 30;
        private static bool isCounting = false;
        private static readonly object lockObj = new object();
        private static HashSet<int> selectedSeats = new HashSet<int>(); // To store selected seat IDs
        private static ApplicationDbContext _context;
        private static ShowtimeSeatApiController showtime = new ShowtimeSeatApiController(_context);

        public CountdownHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task StartCountdown()
        {
            lock (lockObj)
            {
                if (isCounting)
                    return;
                isCounting = true;
            }

            while (countdownTime > 0)
            {
                await Clients.All.SendAsync("ReceiveCountdown", countdownTime);
                await Task.Delay(1000); // Đếm ngược mỗi giây
                countdownTime--;
            }

            await Clients.All.SendAsync("CountdownFinished", selectedSeats); // Send selected seats to clients

            foreach (int seat in selectedSeats)
            {
                await showtime.PutSTSeatStatus(seat, 0);
            }

            ResetCountdown();
        }

        public void SelectSeat(int seatId)
        {
            lock (lockObj)
            {
                if (!selectedSeats.Contains(seatId))
                {
                    selectedSeats.Add(seatId);
                }
            }
        }

        public void DeselectSeat(int seatId)
        {
            lock (lockObj)
            {
                selectedSeats.Remove(seatId);
            }
        }

        private void ResetCountdown()
        {
            lock (lockObj)
            {
                countdownTime = 30;
                isCounting = false;
                selectedSeats.Clear(); // Clear the selected seats after countdown
            }
        }
    }
}