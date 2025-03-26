using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface IShowTimeRepository : IRepository<ShowTime>
    {
        Task<IEnumerable<ShowTime>> GetAllAsync(string? includeProperties = null);
        void Update(ShowTime showTime);
    }
}
