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
    public class WishlistRepository:Repository<Wishlist>, IWishlistRepository
    {
        private readonly ApplicationDbContext _context;
        public WishlistRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
