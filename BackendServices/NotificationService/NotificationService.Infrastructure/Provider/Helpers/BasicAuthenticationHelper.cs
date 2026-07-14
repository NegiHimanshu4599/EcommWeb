using System.Text;

namespace NotificationService.Infrastructure.Provider.Helpers
{
    internal static class BasicAuthenticationHelper
    {
        public static string Create(string apiKey, string apiToken)
        {
            var value = $"{apiKey}:{apiToken}";
            return Convert.ToBase64String( Encoding.UTF8.GetBytes(value));
        }
    }
}
