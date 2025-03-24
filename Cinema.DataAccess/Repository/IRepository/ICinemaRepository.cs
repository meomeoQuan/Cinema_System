using System;

using System.Linq.Expressions;

using System.Threading.Tasks;
using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{

    public interface ICinemaRepository : IRepository<Theater>
    {
        Task<Theater> FindByIdAsync(int id);
        Task AddAsync(Theater theater);

        void Update(Theater theater);
    }
}
