using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Exceptions
{
    public class WishlistException: Exception
    {
        public WishlistException(string message) : base(message)
        {

        }
    }
}
