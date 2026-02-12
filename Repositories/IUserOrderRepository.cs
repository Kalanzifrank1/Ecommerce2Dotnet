using Ecommerce2.Models;

namespace Ecommerce2.Repositories
{
    public interface IUserOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders();
    }
}
