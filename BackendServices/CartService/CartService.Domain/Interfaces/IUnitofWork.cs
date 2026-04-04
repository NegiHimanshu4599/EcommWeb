using CartService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Domain.Interfaces
{
    public interface IUnitofWork
    {
        ICartRepository Cart { get; }
        ICartItemRepository CartItem { get; }
        IWishlistItemRepository WishlistItem { get; }
        IWishlistRepository Wishlist { get; }
        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
