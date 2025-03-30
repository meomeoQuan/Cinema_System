using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Repository
{
    public class RoomRepository : Repository<Room>, IRoomRepository 
    {
        private readonly ApplicationDbContext _db;

        public RoomRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Room room)
        {
            _db.Update(room);
        }

        public async Task<IEnumerable<Room>> GetRoomsByCinemaIdAsync(int cinemaId)
        {
            return await _db.Rooms
                .Include(r => r.Theater)
                .Where(r => r.CinemaID == cinemaId)
                .ToListAsync();
        }
    }

}
