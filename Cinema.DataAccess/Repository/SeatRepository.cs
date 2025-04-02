using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Repository
{
    public class SeatRepository : Repository<Seat>, ISeatRepository 
    {
        private ApplicationDbContext _db;

        public SeatRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<Seat?> GetByIdAsync(int seatId)
        {
            return await _db.Seats.FindAsync(seatId); // Tìm nhanh nhất bằng khóa chính
        }

        public void Update(Seat seat)
        {
            _db.Update(seat);
        }
        public async Task<IEnumerable<Seat>> GetSeatsByRoomIdAsync(int roomId)
        {
            return await _db.Seats
               .Include(s => s.Room)
               .Where(s => s.RoomID == roomId)
               .ToListAsync();
        }
    }
 }


