using CartService.Application.DTOs;
using CartService.Application.Exceptions;
using CartService.Application.Interfaces;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;


namespace CartService.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly ILogger<WishlistService> _logger;
        private readonly HttpClient _httpClient;
        public WishlistService(IHttpClientFactory httpClientFactory ,ILogger<WishlistService> logger, IUnitofWork unitofwork)
        {
            _httpClient = httpClientFactory.CreateClient("BookService");
            _unitofwork = unitofwork;
            _logger = logger;
        }
        public async Task AddAsync(string userId, int bookId)
        {
            var wishlist = await _unitofwork.Wishlist.FirstOrDefaultAsync(x => x.UserId == userId, "WishlistItems");
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId
                };
                await _unitofwork.Wishlist.AddAsync(wishlist);
            }
            var exists = wishlist.WishlistItems.Any(x => x.BookId == bookId);
            if (exists)
            {
                _logger.LogWarning("Book {BookId} already in wishlist", bookId);
                return;
            }
            var response = await _httpClient.GetAsync($"/api/Book/{bookId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException("Book not found");
            }
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ServiceUnavailableException(error);
            }
            wishlist.WishlistItems.Add(new WishlistItem
            {
                BookId = bookId
            });
            wishlist.UpdatedAt = DateTime.UtcNow;
            await _unitofwork.SaveAsync();
        }
        public async Task<WishlistDto> GetAsync(string userId)
        {
            _logger.LogInformation("Fetching wishlist for user {UserId}", userId);
            var wishlist = await _unitofwork.Wishlist.FirstOrDefaultAsync(x => x.UserId == userId, "WishlistItems");
            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist not found for user {UserId}", userId);
                throw new KeyNotFoundException("Wishlist not found For User");
            }
            var bookIds = wishlist.WishlistItems.Select(x => x.BookId).ToList();
            if (!bookIds.Any())
            {
                return new WishlistDto
                {
                    WishlistId = wishlist.Id,
                    UserId = wishlist.UserId,
                    Items = new List<WishlistItemDto>()
                };
            }
            var response = await _httpClient.PostAsJsonAsync("/api/Book/bulk", bookIds);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("BookService error: {Error}", error);
                throw new ServiceUnavailableException(error);
            }
            var books = await response.Content.ReadFromJsonAsync<IEnumerable<BookListDto>>();
            if (books == null)
            {
                throw new ServiceUnavailableException("Book data could not be loaded");
            }
            var booksDictionary = books.ToDictionary(x => x.Id);
            var dto = new WishlistDto
            {
                WishlistId = wishlist.Id,
                UserId = wishlist.UserId,
                Items = wishlist.WishlistItems.Select(x =>
                {
                    booksDictionary.TryGetValue(x.BookId, out var book);

                    return new WishlistItemDto
                    {
                        BookId = x.BookId,
                        Title = book?.Title,
                        Price = book?.Price ?? 0,
                        ImageUrl = book?.ImageUrl
                    };
                }).ToList()
            };
            return dto;
        }
        public async Task RemoveAsync(string userId, int bookId)
        {
            _logger.LogInformation("Removing book {BookId} from wishlist for user {UserId}", bookId, userId);
            var wishlist = await _unitofwork.Wishlist.FirstOrDefaultAsync(x => x.UserId == userId, "WishlistItems");
            if (wishlist == null)
            {
                throw new KeyNotFoundException("Wishlist not found");
            }
            var item = wishlist.WishlistItems.FirstOrDefault(x => x.BookId == bookId);
            if (item != null)
            {
                wishlist.WishlistItems.Remove(item);
                wishlist.UpdatedAt = DateTime.UtcNow;
                await _unitofwork.SaveAsync();
            }
            else
            {
                throw new KeyNotFoundException("Item not Found");
            }
        }
    }
}