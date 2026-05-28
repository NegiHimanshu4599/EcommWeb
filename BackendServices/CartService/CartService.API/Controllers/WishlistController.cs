using CartService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        private string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            return userId;
        }
        [HttpPost("add/{bookId}")]
        public async Task<IActionResult> Add(int bookId)
        {
            var userId = GetUserId();
            await _wishlistService.AddAsync(userId, bookId);
            return Ok("Added to wishlist");
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            var result = await _wishlistService.GetAsync(userId);
            if (result == null)
                return NotFound("Wishlist is empty");
            return Ok(result);
        }
        [HttpDelete("remove/{bookId}")]
        public async Task<IActionResult> Remove(int bookId)
        {
            var userId = GetUserId();
            await _wishlistService.RemoveAsync(userId, bookId);
            return Ok("Removed from wishlist");
        }
    }
}
