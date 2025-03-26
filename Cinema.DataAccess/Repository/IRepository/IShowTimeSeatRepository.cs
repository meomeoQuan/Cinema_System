
using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface IShowTimeSeatRepository : IRepository<ShowtimeSeat>
    {
        void Update(ShowtimeSeat showtimeSeat);
    }
}
