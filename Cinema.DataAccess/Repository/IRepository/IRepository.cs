using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface IRepository <T> where T : class
    {
      
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        Task<IEnumerable<T>> GetAllPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        //Func<object, object> value
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        //=========================================================================================================
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        //=========================================================================================================
       

    
    }
}
