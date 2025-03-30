using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{

    public interface IRoomRepository : IRepository<Room>
    {
        void Update(Room room);
        Task<IEnumerable<Room>> GetRoomsByCinemaIdAsync(int cinemaId);
    }
}
