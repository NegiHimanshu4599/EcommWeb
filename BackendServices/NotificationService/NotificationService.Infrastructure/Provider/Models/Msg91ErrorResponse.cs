using System.Text.Json.Serialization;

namespace NotificationService.Infrastructure.Provider.Models
{
    public sealed class Msg91ErrorResponse
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}