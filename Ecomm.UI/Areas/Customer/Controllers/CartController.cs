using Ecomm.UI.Common;
using Ecomm.UI.Models.CartDto;
using Ecomm.UI.Models.ViewModels;
using Ecomm.UI.ServicesConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ecomm.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IAPIService _apiService;

        public CartController(IAPIService apiService)
        {
            _apiService = apiService;
        }
        public async Task<IActionResult> Index()
        {
            var cart = await _apiService.GetAsync<CartDto>(SD.CartAPIPath);
            if (cart == null)
            {
                cart = new CartDto
                {
                    Items = new List<CartItemDto>()
                };
            }
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details","Home", new { id = dto.BookId });
            }
            try
            {
                await _apiService.PostAsync<object>(SD.CartAPIPath + "/add",dto);
                var cart = await _apiService.GetAsync<CartDto>(SD.CartAPIPath);
                var count = cart?.Items?.Sum(x => x.Quantity) ?? 0;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount,count);
                TempData["success"] = "Book added to cart";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Details", "Home", new { id = dto.BookId });
            }
        }
        public async Task<IActionResult> Plus(int bookId)
        {
            var cart = await _apiService.GetAsync<CartDto>(SD.CartAPIPath);
            var item = cart.Items.FirstOrDefault(x => x.BookId == bookId);
            if (item == null)
                return NotFound();
            await _apiService.PutAsync<CartDto>(SD.CartAPIPath + "/update",new UpdateCartDto
            {
                BookId = item.BookId,
                Quantity = item.Quantity + 1
            });
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int bookId)
        {
            var cart = await _apiService.GetAsync<CartDto>(SD.CartAPIPath);
            var item = cart.Items.FirstOrDefault(x => x.BookId == bookId);
            if (item == null)
                return NotFound();
            if (item.Quantity == 1)
            {
                await _apiService.DeleteAsync(SD.CartAPIPath + "/remove/" + bookId);
            }
            else
            {
                await _apiService.PutAsync<CartDto>(SD.CartAPIPath + "/update",new UpdateCartDto
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity - 1
                });
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int bookId)
        {
            await _apiService.DeleteAsync(SD.CartAPIPath + "/remove/" + bookId);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Clear()
        {
            await _apiService.DeleteAsync(SD.CartAPIPath + "/clear");
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);
            return RedirectToAction(nameof(Index));
        }
    }
}