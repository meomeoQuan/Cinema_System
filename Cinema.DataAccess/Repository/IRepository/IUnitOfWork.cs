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
        public IProductRepository Product { get; }
        void Save();
    }
}
