using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase( this IServiceCollection services, IConfiguration configuration)
        {
            var conStr = configuration.GetConnectionString("cs") ?? throw new InvalidOperationException("Connection String 'cs' is not Found");
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(conStr, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            }));
            return services;
        }
    }
}
