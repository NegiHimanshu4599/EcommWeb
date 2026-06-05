using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Exceptions
{
    public class InsufficientStockException : Exception
    {
        public InsufficientStockException(string Message): base(Message)
        {
        }
    }
}