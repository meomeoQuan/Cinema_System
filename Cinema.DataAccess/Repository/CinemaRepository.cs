using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Repository
{
    public class CinemaRepository : Repository<Theater>, ICinemaRepository
    {
        private readonly ApplicationDbContext _db;

        public CinemaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Theater> FindByIdAsync(int cinemaId)
        {
            return await _db.Cinemas.FindAsync(cinemaId);
        }
        // Thêm rạp mới vào database
        public async Task AddAsync(Theater theater)
        {
            await _db.Cinemas.AddAsync(theater);
        }

        // Cập nhật thông tin rạp
        public void Update(Theater theater)
        {
            _db.Cinemas.Update(theater);
        }
    }
}
