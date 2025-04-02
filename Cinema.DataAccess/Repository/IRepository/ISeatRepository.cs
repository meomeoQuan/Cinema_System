using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface ISeatRepository : IRepository<Seat>
    {
        void Update(Seat seat);
        Task<IEnumerable<Seat>> GetSeatsByRoomIdAsync(int roomId);
        Task<Seat?> GetByIdAsync(int seatId); // Thêm phương thức tối ưu
    }

}
