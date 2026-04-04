using CartService.Infrastructure.Data;
using CartService.Infrastructure.Repository;
using CartService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Domain.Entities;

namespace CartService.Infrastructure.Repository
{
    public class WishlistItemRepository:Repository<WishlistItem>, IWishlistItemRepository
    {
        private readonly ApplicationDbContext _context;
        public WishlistItemRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
