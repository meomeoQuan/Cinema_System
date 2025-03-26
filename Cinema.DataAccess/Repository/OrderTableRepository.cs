using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
    public class OrderTableRepository : Repository<OrderTable>, IOrderTableRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderTableRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(OrderTable orderTable)
        {
           _db.Update(orderTable);
        }
    }
}
