using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;

namespace Cinema.DataAccess.Repository
{
    public class ShowTimeRepository : Repository<ShowTime>, IShowTimeRepository
    {
        private readonly ApplicationDbContext _db;
        public ShowTimeRepository(ApplicationDbContext context) : base(context)
        {
            _db = context;
        }

        public async Task<IEnumerable<ShowTime>> GetAllAsync(string? includeProperties = null)
        {
            IQueryable<ShowTime> query = _db.showTimes;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.ToListAsync();
        }

        public void Update(ShowTime showTime)
        {
            _db.Update(showTime);
        }

        //public void Update(ShowTime showTime)
        //{
        //    var objFromDb = _db.showTimes.FirstOrDefault(s => s.ShowTimeID == showTime.ShowTimeID);
        //    if (objFromDb != null)
        //    {
        //        objFromDb.ShowDates = showTime.ShowDates;
        //        objFromDb.ShowTimes = showTime.ShowTimes;
        //        objFromDb.CinemaID = showTime.CinemaID;
        //        objFromDb.MovieID = showTime.MovieID;
        //        objFromDb.RoomID = showTime.RoomID;
        //    }
        //}
    }
}
