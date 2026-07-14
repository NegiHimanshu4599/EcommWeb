using System.Text.Json.Serialization;

namespace NotificationService.Infrastructure.Provider.Models
{
    public sealed class Msg91SmsRequest
    {
        [JsonPropertyName("template_id")]
        public string TemplateId { get; set; } = null!;
        [JsonPropertyName("short_url")]
        public string ShortUrl { get; set; } = "0";
        [JsonPropertyName("recipients")]
        public List<Msg91Recipient> Recipients { get; set; } = new();
    }
    public sealed class Msg91Recipient
    {
        [JsonPropertyName("mobiles")]
        public string MobileNumber { get; set; } = null!;
        [JsonPropertyName("VAR1")]
        public string? Variable1 { get; set; }
        [JsonPropertyName("VAR2")]
        public string? Variable2 { get; set; }
        [JsonPropertyName("VAR3")]
        public string? Variable3 { get; set; }
    }
}