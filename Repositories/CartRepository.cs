using Ecommerce2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom;

namespace Ecommerce2.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartRepository(
            ApplicationDbContext db, 
            IHttpContextAccessor httpContextAccessor, 
            UserManager<IdentityUser> userManager
            )
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        //Getting users cart
        public async Task<ShoppingCart> GetUserCart()
        {
            var userId = GetUserId();
            if(userId == null) 
            {
                throw new Exception("Invalid userId");
            }
            var shoppingCart = await _db.ShoppingCarts
                                        .Include(a => a.CartDetail)
                                        .ThenInclude(a => a.Book)
                                        .ThenInclude(a => a.Genre)
                                        .Where(a => a.UserId == userId).FirstOrDefaultAsync();
            return shoppingCart;
        }

        public async Task<int> AddItem(int bookId, int qty)
        { 
            string userId = GetUserId();

            using var transaction = _db.Database.BeginTransaction();
            try
            {
                

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("User is not logged in");
                }

                var cart = await GetCart(userId);
                if (cart is null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId
                    };
                    _db.ShoppingCarts.Add(cart);
                }
                _db.SaveChanges();
                //Cart details section
                var cartItem = _db.CartDetails?.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.Book.Id == bookId);
                if (cartItem != null)
                {
                    cartItem.Quanatity = qty;
                }
                else {
                    var book = _db.Books.Find(bookId);
                    cartItem = new CartDetail
                    {
                        BookId = bookId,
                        ShoppingCartId = cart.Id,
                        Quanatity = qty,
                        UnitPrice = book.Price
                    };
                    _db.CartDetails.Add(cartItem);
                }
                _db.SaveChanges();
                transaction.Commit();
                
            }catch(Exception ex)
            {
                throw new Exception("Failed to add item");
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<int> RemoveItem(int bookId)
        {
            string userId = GetUserId();
            //using var transaction = _db.Database.BeginTransaction();
            try
            {

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("User not found");
                }

                var cart = await GetCart(userId);
                
                if (cart is null)
                {
                    throw new Exception($"{userId} is not a cart");
                }
                _db.SaveChanges();
                //Cart details section
                var cartItem = _db.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.Book.Id == bookId);
                if (cartItem is null) {
                    throw new Exception("");
                }
                else if(cartItem.Quanatity == 1)
                {
                    _db.CartDetails.Remove(cartItem);
                }
                else
                {
                    cartItem.Quanatity = cartItem.Quanatity - 1;
                }
                _db.SaveChanges();
                //transaction.Commit();
               
            }
            catch (Exception ex)
            {
            }
            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<int> GetCartItemCount(string userId="")
        {
            if (string.IsNullOrEmpty(userId)) { 
                userId = GetUserId();
            }
            try
            {
                var data = await (from cart in _db.CartDetails
                                  join CartDetail in _db.CartDetails
                                  on cart.Id equals CartDetail.ShoppingCartId
                                  where cart.UserId == userId
                                  select new { CartDetail.Id }
                              ).ToListAsync();
                return data.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve count", ex);
            }
        }

        public async Task<ShoppingCart> GetCart(string userId)
        {
            var cart = await _db.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<bool> DoCheckOut(ChechoutModel model)
        {
            using var transaction = _db.Database.BeginTransaction();
            try {
                //move data from cartDetail to order detai; then will remove cart detail
                //entry->order, orderdatail
                //remove data->cartDetail
                var userId = GetUserId();

                if (string.IsNullOrEmpty(userId)) {
                    throw new Exception("User is not logged in");
                }
                var cart = await GetCart(userId);
                if (cart is null) {
                    throw new Exception("Invalid cart");
                }
                var cartDetail = _db.CartDetails
                                        .Where(a => a.ShoppingCartId == cart.Id).ToList();
                if (cartDetail.Count == 0) {
                    throw new Exception("Cart is empty");
                }
                var pendingRecord = _db.OrderStatuses.FirstOrDefault(s => s.StatusName == "Pending");
                if (pendingRecord is null) 
                {
                    throw new Exception("Order status does not have Pending status");
                }
                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    //name = model.Name,
                    Email = model.Email,
                    MobileNuumber = model.PhoneNumber,
                    //PaymentMethod = model.PaymentMethods,
                    IsPaid= false,
                    OrderStatusId = pendingRecord.Id, // Pending
                };
                _db.Orders.Add(order);
                _db.SaveChanges();

                foreach(var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        BookId = item.BookId,
                        OrderId = order.Id,
                        Quantity = item.Quanatity,
                        UnitPrice = item.UnitPrice
                    };
                    _db.OrderDetails.Add(orderDetail);
                }
                _db.SaveChanges();
                //romoving cart details
                _db.CartDetails.RemoveRange(cartDetail);
                _db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*
        Pseudocode / Plan:
        1. Safely obtain HttpContext from _httpContextAccessor.
        2. If HttpContext is null, return empty string to avoid null propagation.
        3. Obtain ClaimsPrincipal (User) from HttpContext.
        4. If ClaimsPrincipal is null, return empty string.
        5. Call _userManager.GetUserId(principal) which may return null.
        6. Return the non-null string by using null-coalescing to empty string.
        7. This guarantees a non-null string return and removes nullable warnings
        (CS8602, CS8600, CS8603) without changing callers.
        */

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
