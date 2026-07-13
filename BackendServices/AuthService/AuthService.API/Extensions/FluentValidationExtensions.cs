using AuthService.Application.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace AuthService.API.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
            return services;
        }
    }
}
