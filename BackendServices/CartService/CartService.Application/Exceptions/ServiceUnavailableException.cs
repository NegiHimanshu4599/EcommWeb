using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Exceptions
{
    public class ServiceUnavailableException: Exception
    {
        public ServiceUnavailableException(string Message) : base(Message) 
        {
        }
    }
}
