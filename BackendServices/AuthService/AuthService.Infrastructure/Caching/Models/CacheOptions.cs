using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Caching.Models
{
    public class CacheOptions
    {
        public string ConnectionString { get; set; }
        public string InstanceName { get; set; }
    }
}
