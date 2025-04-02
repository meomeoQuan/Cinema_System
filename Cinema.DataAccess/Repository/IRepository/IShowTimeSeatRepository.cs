
using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface IShowTimeSeatRepository : IRepository<ShowtimeSeat>
    {
        void Update(ShowtimeSeat showtimeSeat);
        Task<IEnumerable<ShowtimeSeat>> AddRangeAsync(List<ShowtimeSeat> showtimeSeats);
    }
}
