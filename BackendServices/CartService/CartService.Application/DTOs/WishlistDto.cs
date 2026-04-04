using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.DTOs
{
    public class WishlistDto
    {
        public int WishlistId { get; set; }
        public string UserId { get; set; }
        public List<int> BookIds { get; set; }
    }
}
