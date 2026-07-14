using AuthService.Application.Interfaces;
using AuthService.Application.Mappings;
using AuthService.Application.Security;
using AuthService.Application.Services;
using AuthService.Domain.Interface;
using AuthService.Infrastructure.Repository;

namespace AuthService.API.Extensions;
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        // Application Services
        services.AddScoped<IAuthService, AuthService.Application.Services.AuthService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        // Infrastructure Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        // Framework
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddHttpContextAccessor();
        return services;
    }
}