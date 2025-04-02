using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>, IDisposable
    {
        void Update(Product product);
        IEnumerable<Product> GetAll();

        IEnumerable<Product> GetProductsByType(ProductType type);

        Product GetFirstOrDefault(
            Expression<Func<Product, bool>> filter = null,
            string includeProperties = null,
            bool tracking = true);

        Product GetById(int id);

        Task<Product> GetFirstOrDefaultAsync(
            Expression<Func<Product, bool>> filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
            string includeProperties = null,
            bool tracking = true);

        Task<Product> GetByIdAsync(int id);

        int Count(Expression<Func<Product, bool>> filter = null);

        //IEnumerable<Product> GetPaged(
        //    int pageNumber,
        //    int pageSize,
        //    Expression<Func<Product, bool>> filter = null,
        //    Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
        //    string includeProperties = null);
    }
}