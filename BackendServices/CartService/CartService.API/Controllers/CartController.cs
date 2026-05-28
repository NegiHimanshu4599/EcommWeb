using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        private string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            return userId;
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
            var userId = GetUserId();
            var result = await _cartService.AddToCartAsync(userId, dto);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var result = await _cartService.GetCartAsync(userId);
            if (result == null)
                return NotFound("Cart is empty");
            return Ok(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity(UpdateCartDto dto)
        {
            var userId = GetUserId();
            var result = await _cartService.UpdateQuantityAsync(userId, dto);
            return Ok(result);
        }
        [HttpDelete("remove/{bookId}")]
        public async Task<IActionResult> RemoveItem(int bookId)
        {
            var userId = GetUserId();
            await _cartService.RemoveItemAsync(userId, bookId);
            return Ok("Item removed");
        }
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _cartService.ClearCartAsync(userId);
            return Ok("Cart cleared");
        }
    }
}
