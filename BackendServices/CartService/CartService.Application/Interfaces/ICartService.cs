using CartService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> AddToCartAsync(string userId, AddToCartDto dto);
        Task<CartDto> GetCartAsync(string userId);
        Task<CartDto> UpdateQuantityAsync(string userId, UpdateCartDto dto);
        Task RemoveItemAsync(string userId, int bookId);
        Task ClearCartAsync(string userId);
    }
}