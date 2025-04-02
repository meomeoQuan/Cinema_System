using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository, IDisposable
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            _db.Products.Update(product);
        }

        public Product GetFirstOrDefault(
            Expression<Func<Product, bool>> filter = null,
            string includeProperties = null,
            bool tracking = true)
        {
            IQueryable<Product> query = _db.Products;

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(
                    new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return query.FirstOrDefault();
        }

        public async Task<Product> GetFirstOrDefaultAsync(
            Expression<Func<Product, bool>> filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
            string includeProperties = null,
            bool tracking = true)
        {
            IQueryable<Product> query = _db.Products;

            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(
                    new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.FirstOrDefaultAsync();
        }

        public IEnumerable<Product> GetProductsByType(ProductType type)
        {
            return _db.Products.Where(p => p.ProductType == type).ToList();
        }

        public IEnumerable<Product> GetAll()
        {
            return _db.Products.ToList();
        }

        // Thêm các phương thức bổ sung
        public Product GetById(int id)
        {
            return _db.Products.Find(id);
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _db.Products.FindAsync(id);
        }

        public int Count(Expression<Func<Product, bool>> filter = null)
        {
            return filter == null
                ? _db.Products.Count()
                : _db.Products.Count(filter);
        }

        public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}