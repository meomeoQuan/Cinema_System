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
    public class ShowTimeSeatRepository : Repository<ShowtimeSeat>, IShowTimeSeatRepository
    {
        private readonly ApplicationDbContext _db;

        public ShowTimeSeatRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public void Update(ShowtimeSeat showtimeSeat)
        {
            _db.Update(showtimeSeat);
        }
    }
}
