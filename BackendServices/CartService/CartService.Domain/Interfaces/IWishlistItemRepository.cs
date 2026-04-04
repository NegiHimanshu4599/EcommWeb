using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Domain.Interfaces
{
    public interface IWishlistItemRepository:IRepository<WishlistItem>
    {
    }
}
