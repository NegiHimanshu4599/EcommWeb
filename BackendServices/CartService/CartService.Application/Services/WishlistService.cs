using AutoMapper;
using CartService.Application.DTOs;
using CartService.Application.Interfaces;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitofWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly ILogger<WishlistService> _logger;
        public WishlistService(ILogger<WishlistService>logger,IMapper mapper,IUnitofWork unitofwork)
        {
            _unitofwork = unitofwork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task AddAsync(string userId, int bookId)
        {
            var wishlist = await _unitofwork.Wishlist.FirstOrDefaultAsync(x => x.UserId == userId, "WishlistItems");
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
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
            wishlist.WishlistItems.Add(new WishlistItem
            {
                BookId = bookId
            });
            await _unitofwork.SaveAsync();
        }
        public async Task<WishlistDto> GetAsync(string userId)
        {
            _logger.LogInformation("Fetching wishlist for user {UserId}", userId);
            var wishlist = await _unitofwork.Wishlist.FirstOrDefaultAsync(x => x.UserId == userId, "WishlistItems");
            if (wishlist == null)
            {
                _logger.LogWarning("Wishlist not found for user {UserId}", userId);
                return null;
            }
            var dto = new WishlistDto
            {
                WishlistId = wishlist.Id,
                UserId = wishlist.UserId,
                BookIds = wishlist.WishlistItems
                    .Select(x => x.BookId)
                    .ToList()
            };
            return dto;
        }
        public async Task RemoveAsync(string userId, int bookId)
        {
            _logger.LogInformation("Removing book {BookId} from wishlist for user {UserId}", bookId, userId);
            var wishlist = await _unitofwork.Wishlist.FirstOrDefaultAsync(x => x.UserId == userId, "WishlistItems");
            var item = wishlist?.WishlistItems.FirstOrDefault(x => x.BookId == bookId);
            if (item != null)
            {
                wishlist.WishlistItems.Remove(item);
                await _unitofwork.SaveAsync();
            }
            else
            {
                _logger.LogWarning("Book {BookId} not found in wishlist", bookId);
            }
        }
    }
}
