using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Caching.Constants
{
    public static class CacheKeys
    {
        public static string UserProfile(string id) => $"user-profile:{id}";
        public static string UserRoles(string id) => $"user-roles:{id}";
        public static string RefreshToken(string token) => $"refresh-token:{token}";
    }
}
