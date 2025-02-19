using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IMovieRepository Movie { get; }
        public ICouponRepository Coupon { get; }
        public IProductRepository Product { get; }
         Task SaveAsync();
    }
}
