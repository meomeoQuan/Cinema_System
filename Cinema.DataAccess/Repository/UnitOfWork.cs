using System;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;

namespace Cinema.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Product = new ProductRepository(_db);
            Movie = new MovieRepository(_db);
            Coupon = new CouponRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            showTime = new ShowTimeRepository(_db);
        }

        public IMovieRepository Movie { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICouponRepository Coupon { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }

        public IShowTimeRepository showTime { get; private set; }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

       
    }
}
