using CartService.Infrastructure.Data;
using CartService.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;

namespace CartService.Infrastructure.Repository
{
    public  class CartItemRepository:Repository<CartItem>,ICartItemRepository
    {
        private readonly ApplicationDbContext _context;
        public CartItemRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
    }
}
