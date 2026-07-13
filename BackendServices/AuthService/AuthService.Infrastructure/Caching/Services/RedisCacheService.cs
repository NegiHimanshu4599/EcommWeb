using AuthService.Infrastructure.Caching.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Caching.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IRedisAsync _redis;
        public RedisCacheService(IRedisAsync redis)
        {
            _redis = redis;
        }
        public Task<bool> ExistsAsync(string key)
        {
            throw new NotImplementedException();
        }
        public Task<T?> GetAsync<T>(string key)
        {
            throw new NotImplementedException();
        }
        public Task RemoveAsync(string key)
        {
            throw new NotImplementedException();
        }
        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            throw new NotImplementedException();
        }
    }
}