using AutoMapper;
using CartService.Application.DTOs;
using CartService.Application.Exceptions;
using CartService.Application.Interfaces;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace CartService.Application.Services
{
    public class CartServices : ICartService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly ILogger<CartServices> _logger;
        private readonly HttpClient _httpClient;
        public CartServices(IHttpClientFactory httpClientFactory , IUnitofWork unitofwork,ILogger<CartServices>logger, IMapper mapper)
        {
            _httpClient = httpClientFactory.CreateClient("BookService");
            _unitofwork = unitofwork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto)
        {
            var cart = await _unitofwork.Cart.FirstOrDefaultAsync(x => x.UserId == userId, "CartItems");
            if (cart == null)
            {
                cart = new Cart
                {
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId
                };
                await _unitofwork.Cart.AddAsync(cart);
            }
            var response = await _httpClient.GetAsync($"/api/Book/{dto.BookId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("BookService failed for BookId {BookId}", dto.BookId);
                throw new KeyNotFoundException("Book not found");
            }
            var book = await response.Content.ReadFromJsonAsync<BookListDto>();
            if (book == null)
            {
                _logger.LogWarning("Book data null for BookId {BookId}", dto.BookId);
                throw new KeyNotFoundException("Book data null");
            }
            var existingItem = cart.CartItems.FirstOrDefault(x => x.BookId == dto.BookId);
            var totalQuantity = existingItem != null ? existingItem.Quantity + dto.Quantity: dto.Quantity;
            if (totalQuantity > book.StockQuantity)
            {
                throw new InsufficientStockException("Not enough stock available");
            }
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    BookId = dto.BookId,
                    Quantity = dto.Quantity,
                    Price = book.Price
                });
            }
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitofwork.SaveAsync();
            return await GetCartAsync(userId);
        }
        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await _unitofwork.Cart.FirstOrDefaultAsync(x => x.UserId == userId, "CartItems");
            if (cart == null)
            {
                _logger.LogWarning("Returning null cart for user {UserId} because no cart was found", userId);
                return new CartDto
                {
                    UserId = userId,
                    Items = new List<CartItemDto>(),
                    TotalPrice = 0
                };
            }
            var cartDto = _mapper.Map<CartDto>(cart);
            var bookIds = cart.CartItems.Select(x => x.BookId).ToList();
            if (!bookIds.Any())
            {
                return cartDto;
            }
            var response = await _httpClient.PostAsJsonAsync("/api/Book/bulk", bookIds);
            if (!response.IsSuccessStatusCode)
            {
                //throw new ServiceUnavailableException("BookService failed");
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("BookService error: {Error}", error);
                throw new ServiceUnavailableException(error);
            }
            var bookList = await response.Content.ReadFromJsonAsync<IEnumerable<BookListDto>>();
            if (bookList == null || !bookList.Any())
            {
                _logger.LogWarning("No book data returned from BookService");
                throw new KeyNotFoundException("No book details found");
            }
            cartDto.Items = cart.CartItems.Select(x =>
            {
                var book = bookList.FirstOrDefault(b => b.Id == x.BookId);
                if (book == null)
                {
                    _logger.LogWarning("Book {BookId} missing in BookService response", x.BookId);
                    throw new KeyNotFoundException($"Book {x.BookId} missing");
                }
                return new CartItemDto
                {
                    BookId = x.BookId,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ProductName = book?.Title,
                    ImageUrl = book?.ImageUrl
                };
            }).ToList();
            cartDto.TotalPrice = cart.CartItems.Sum(x => x.Price * x.Quantity);
            return cartDto;
        }
        public async Task<CartDto> UpdateQuantityAsync(string userId, UpdateCartDto dto)
        {
            var cart = await _unitofwork.Cart.FirstOrDefaultAsync(x => x.UserId == userId, "CartItems");
            if(cart == null)
            {
                _logger.LogWarning("Cart not found for user {UserId} when updating quantity", userId);
                throw new KeyNotFoundException("Cart not found");
            }
            var item = cart.CartItems.FirstOrDefault(x => x.BookId == dto.BookId);
            if (item == null)
            {
                _logger.LogWarning("Item with book ID {BookId} not found in cart for user {UserId}", dto.BookId, userId);
                throw new KeyNotFoundException("Item not found");
            }
            var response = await _httpClient.GetAsync($"/api/Book/{dto.BookId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("BookService failed for BookId {BookId}", dto.BookId);
                throw new KeyNotFoundException("Book not found");
            }
            var book =await response.Content.ReadFromJsonAsync<BookListDto>();
            if(book == null)
            {
                _logger.LogWarning("No book data returned from BookService");
                throw new KeyNotFoundException("No book details found");
            }
            if (dto.Quantity > book.StockQuantity)
            {
                throw new InsufficientStockException("Not enough stock available");
            }
            item.Quantity = dto.Quantity;
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitofwork.SaveAsync();
            return await GetCartAsync(userId);
        }
        public async Task RemoveItemAsync(string userId, int bookId)
        {
            var cart = await _unitofwork.Cart.FirstOrDefaultAsync(x => x.UserId == userId, "CartItems");
            if (cart == null)
                throw new KeyNotFoundException("Cart not found");
            var item = cart.CartItems.FirstOrDefault(x => x.BookId == bookId);
            if (item == null)
                throw new KeyNotFoundException("Item not found");
            cart.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitofwork.SaveAsync();            
        }
        public async Task ClearCartAsync(string userId)
        {
            var cart = await _unitofwork.Cart.FirstOrDefaultAsync(x => x.UserId == userId, "CartItems");
            if (cart != null)
            {
                _logger.LogInformation("Clearing all items from cart for user {UserId}", userId);
                cart.CartItems.Clear();
                cart.UpdatedAt = DateTime.UtcNow;
                await _unitofwork.SaveAsync();
            }
        }
    }
}