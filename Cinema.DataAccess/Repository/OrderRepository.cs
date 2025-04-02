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
    public class OrderRepository : Repository<OrderTable>, IOrderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }
        public async Task<double> GetTotalRevenueAsync()
        {
            return await _db.OrderTables.SumAsync(o => o.TotalAmount);
        }
        public async Task<int> GetCountAsync()
        {
            return await _db.OrderTables.CountAsync();
        }

        public void Update(OrderTable orderTable)
        {
            _db.Update(orderTable);
        }

    }
}
