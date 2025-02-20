using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;

namespace Cinema.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Product = new ProductRepository(_db);
            Movie = new MovieRepository(_db);
            Coupon = new CouponRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);

        }
        public IMovieRepository Movie { get; private set; }

        public IProductRepository Product { get; private set; }

        public ICouponRepository Coupon { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
