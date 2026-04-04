using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.DTOs
{
    public class AddToCartDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
