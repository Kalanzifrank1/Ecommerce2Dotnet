using Ecommerce2.Models;

namespace Ecommerce2.Repositories
{
    public interface ICartRepository
    {
        Task<ShoppingCart> GetUserCart();
        Task<int> AddItem(int bookId, int qty);
        Task<int> RemoveItem(int bookId);
        Task<int> GetCartItemCount(string userId = "");
        string GetUserId();
    }
}
