using System.Text.Json.Serialization;

namespace NotificationService.Infrastructure.Provider.Models
{
    public sealed class ExotelErrorResponse
    {
        [JsonPropertyName("Message")]
        public string? Message { get; set; }
        [JsonPropertyName("Code")]
        public string? Code { get; set; }
    }
}