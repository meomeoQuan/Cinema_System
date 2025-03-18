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
    public class CinemaRepository : Repository<Theater>, ICinemaRepository 
    {
        private ApplicationDbContext _db;

        public CinemaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Theater theater)
        {
            _db.Update(theater);
        }

    }

}
