using CartService.Infrastructure.Data;
using CartService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Infrastructure.Repository
{
    public class UnitofWork : IUnitofWork
    {
        private readonly ApplicationDbContext _context;
        public UnitofWork(ApplicationDbContext context)
        {
            _context = context;
                Cart = new CartRepository(context);
                CartItem = new CartItemRepository(context);
                WishlistItem = new WishlistItemRepository(context);
                Wishlist = new WishlistRepository(context);
        }
        public ICartRepository Cart { private set; get; }
        public ICartItemRepository CartItem { private set; get; }
        public IWishlistItemRepository WishlistItem { private set; get; }
        public IWishlistRepository Wishlist { private set; get; }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
