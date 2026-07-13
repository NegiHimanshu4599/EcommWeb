using System.Text.Json.Serialization;

namespace NotificationService.Infrastructure.Provider.Models
{
    public sealed class ExotelVoiceRequest
    {
        [JsonPropertyName("From")]
        public string From { get; set; } = null!;
        [JsonPropertyName("To")]
        public string To { get; set; } = null!;
        [JsonPropertyName("CallerId")]
        public string CallerId { get; set; } = null!;
        [JsonPropertyName("Url")]
        public string Url { get; set; } = null!;
        public string TimeLimit { get; set; }
        public string TimeOut { get; set; }
        public string StatusCallback { get; set; }
        public string CallType { get; set; }
        public string CustomeField { get; set; }
    }
}