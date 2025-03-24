using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
    public class SeatRepository : Repository<Seat>, ISeatRepository 
    {
        private ApplicationDbContext _db;

        public SeatRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Seat seat)
        {
            _db.Update(seat);
        }

    }

}
