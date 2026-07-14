using System.Text.Json;

namespace NotificationService.Infrastructure.Provider.Helpers
{
    internal static class JsonSerializerOptionsProvider
    {
        public static readonly JsonSerializerOptions Default = new(JsonSerializerDefaults.Web);
    }   
}
