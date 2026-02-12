using Ecommerce2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce2.Repositories
{
    public class UserOrderRepository : IUserOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserOrderRepository(
            ApplicationDbContext db,
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager
            )
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<IEnumerable<Order>> UserOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged in");
            var orders = await _db.Orders
                                .Include(x => x.OrderStatus)
                                .Include(x => x.OrderDetails)
                                .ThenInclude(x => x.Book)
                                .ThenInclude(x => x.Genre)
                                .Where(a => a.UserId == userId)
                                .ToListAsync();
            return orders;
        }

        public string GetUserId()
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext == null)
            {
                return string.Empty;
            }

            var principal = httpContext.User;
            if (principal == null)
            {
                return string.Empty;
            }

            string? userId = _userManager.GetUserId(principal);
            return userId ?? string.Empty;
        }
    }
}
