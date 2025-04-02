using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Cinema.DataAccess.Data;
using Cinema.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;

namespace Cinema.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();

        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

 
            if (filter != null)
            {
                query = query.Where(filter);
            }

 
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.ToListAsync();
        }


        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public async Task<IEnumerable<T>> GetAllPagedAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, int pageIndex = 1, int pageSize = 3)
        {
            IQueryable<T> query = dbSet;

            // Apply filtering
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include related tables (if any)
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            // Apply paging
            return await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;


            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        // offset    movies = 10 -> page 1 -> offset 0 , movies 3 
        // page 2 -> offset 3 , movies 3
        // page 3 -> offset 6 , movies 3
        // page 4 -> offset 9 , movies 1

        // 
        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            if (filter != null)
            {
                return await dbSet.CountAsync(filter);
            }
            return await dbSet.CountAsync();
        }


        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            // Category,CategoryID ->    {"Category","CategoryID"}
            return query.FirstOrDefault();
        }


        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query.Where(filter);

            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.
                   Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.ToList();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }
    }

}
