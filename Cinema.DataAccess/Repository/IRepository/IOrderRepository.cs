using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<OrderTable>
    {
        void Update(OrderTable orderTable);
        Task<double> GetTotalRevenueAsync();
        Task<int> GetCountAsync();

    }
}
