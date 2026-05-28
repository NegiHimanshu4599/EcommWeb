using CartService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Interfaces
{
    public interface IWishlistService
    {
        Task AddAsync(string userId, int bookId);
        Task RemoveAsync(string userId, int bookId);
        Task<WishlistDto> GetAsync(string userId);
    }
}
