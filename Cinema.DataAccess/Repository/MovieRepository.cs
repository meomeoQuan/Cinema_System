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
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _db;
        public MovieRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Movie movie)
        {
            _db.Update(movie);
        }

        public async Task<IEnumerable<Movie>> SearchAsync(string searchTerm, bool? isUpcoming = null)
        {
            try
            {
                var query = _db.Movies.AsQueryable();

                if (isUpcoming.HasValue)
                {
                    query = query.Where(m => m.IsUpcomingMovie == isUpcoming.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(m =>
                        m.Title.Contains(searchTerm) ||
                        (!string.IsNullOrEmpty(m.Synopsis) && m.Synopsis.Contains(searchTerm)));
                }

                return await query.ToListAsync() ?? new List<Movie>();
            }
            catch
            {
                return new List<Movie>();
            }
        }

        public async Task<IEnumerable<Movie>> SearchAsync(string searchTerm, bool? isUpcoming = null)
        {
            try
            {
                var query = _db.Movies.AsQueryable();

                if (isUpcoming.HasValue)
                {
                    query = query.Where(m => m.IsUpcomingMovie == isUpcoming.Value);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(m =>
                        m.Title.Contains(searchTerm) ||
                        (!string.IsNullOrEmpty(m.Synopsis) && m.Synopsis.Contains(searchTerm)));
                }

                return await query.ToListAsync() ?? new List<Movie>();
            }
            catch
            {
                return new List<Movie>();
            }
        }
    }
}
