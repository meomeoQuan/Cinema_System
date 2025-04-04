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
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private readonly ApplicationDbContext _db;

        public CouponRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Coupon coupon)
        {
            _db.Update(coupon);
        }

        public async Task AddAsync(Coupon coupon)
        {
            await _db.Coupons.AddAsync(coupon);
        }
    }
}
