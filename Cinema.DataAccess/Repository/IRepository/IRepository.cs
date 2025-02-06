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

       
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);

        IEnumerable<T> GetAll(Expression<Func<T,bool>> ? filter = null, string ? includeProperties = null);


        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
