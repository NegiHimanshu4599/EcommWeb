using AuthService.Infrastructure.Caching.Interfaces;
using AuthService.Infrastructure.Caching.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Caching.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["Redis:ConnectionString"];
            options.InstanceName = configuration["Redis:InstanceName"];
        });
        services.AddScoped<ICacheService, RedisCacheService>();
        return services;
    }
}