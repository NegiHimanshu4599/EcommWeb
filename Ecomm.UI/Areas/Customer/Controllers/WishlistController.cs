using Ecomm.UI.Common;
using Ecomm.UI.Models.WishlistDto;
using Ecomm.UI.ServicesConnection;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.UI.Areas.Customer.Controllers
{

    [Area("Customer")]
    public class WishlistController : Controller
    {
        private readonly IAPIService _apiService;

        public WishlistController(IAPIService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult>Index()
        {
            var wishlist = await _apiService.GetAsync<WishlistDto>(SD.WishlistAPIPath);
            return View(wishlist);
        }
        [HttpPost]
        public async Task<IActionResult> Add(int bookId)
        {
            try
            {
                await _apiService.PostAsync<object>(
                    $"{SD.WishlistAPIPath}/add/{bookId}",
                    new { });
                TempData["success"] = "Book added to wishlist";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Already"))
                {
                    TempData["warning"] = "Book already exists in wishlist";
                }
                else
                {
                    TempData["error"] = "Something went wrong. Please try again.";
                }
            }
            return RedirectToAction("Details", "Home", new { id = bookId });

        }
        [HttpPost]
        public async Task<IActionResult>Remove(int bookId)
        {
            await _apiService.DeleteAsync($"{SD.WishlistAPIPath}/remove/{bookId}");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult>MoveToCart(int bookId)
        {
            await _apiService.PostAsync<object>($"{SD.CartAPIPath}/add",new
            {
                BookId = bookId,
                Quantity = 1
            });
            await _apiService.DeleteAsync($"{SD.WishlistAPIPath}/remove/{bookId}");
            return RedirectToAction(nameof(Index));
        }
    }
}