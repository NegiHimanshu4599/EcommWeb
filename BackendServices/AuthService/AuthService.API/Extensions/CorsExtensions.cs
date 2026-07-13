namespace AuthService.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicies(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowMVC", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            return services;
        }
    }
}