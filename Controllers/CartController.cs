using Ecommerce2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ecommerce2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        public CartController(ICartRepository cartRepo)
        {
            _cartRepository = cartRepo;
        }

        public async Task<IActionResult> AddItem(int bookId, int qty=1, int redirect=0)
        {
            var cartCount = await _cartRepository.AddItem(bookId, qty);
            if (redirect == 0)
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var cartCount = await _cartRepository.RemoveItem(bookId);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart() {
            var cart = await _cartRepository.GetUserCart();
            return View(cart);
        }
        public async Task<IActionResult> GetTotalItemsInCart() { 
            int cartItem = await _cartRepository.GetCartItemCount();
            return Ok(cartItem);
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(ChechoutModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }
            bool isCheckedOut = await _cartRepository.DoCheckOut(model);
            if (!isCheckedOut)
                return RedirectToAction(nameof(OrderFailure));
            return RedirectToAction(nameof(OrderSuccess));
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult OrderFailure()
        {
            return View();
        }
    }
}
