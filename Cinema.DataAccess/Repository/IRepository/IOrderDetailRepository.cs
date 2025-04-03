using Cinema.Models;

namespace Cinema.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail OrderDetail);

    }
}
