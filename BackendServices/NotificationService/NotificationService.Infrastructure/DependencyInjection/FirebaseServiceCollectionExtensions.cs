using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Domain.Configuration;

namespace NotificationService.Infrastructure.DependencyInjection
{
    public static class FirebaseServiceCollectionExtensions
    {
        public static IServiceCollection AddFirebase(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<FirebaseSettings>(configuration.GetSection(FirebaseSettings.SectionName));
            var settings = configuration.GetSection(FirebaseSettings.SectionName).Get<FirebaseSettings>()
                ?? throw new InvalidOperationException("FirebaseSettings configuration is missing.");
            if (string.IsNullOrWhiteSpace(settings.ServiceAccountPath))
                throw new InvalidOperationException("Firebase ServiceAccountPath is missing.");

            if (!File.Exists(settings.ServiceAccountPath))
                throw new FileNotFoundException($"Firebase credential file not found: {settings.ServiceAccountPath}");
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(settings.ServiceAccountPath),
                    ProjectId = settings.ProjectId
                });
            }
            return services;
        }
    }
}