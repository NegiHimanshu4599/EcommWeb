using System.ComponentModel.DataAnnotations;

namespace NotificationService.Domain.Configuration
{
    public sealed class VoiceCallSettings
    {
        public const string SectionName = "VoiceCallSettings";
        [Required]
        public string BaseUrl { get; set; } = null!;
        [Required]
        public string ApiKey { get; set; } = null!;
        [Required]
        public string ApiToken { get; set; } = null!;
        [Required]
        public string AccountSid { get; set; } = null!;
        [Required]
        public string CallerId { get; set; } = null!;
        // Exotel App / Flow ID
        [Required]
        public string AppId { get; set; } = null!;
        // Public URL of your NotificationService
        [Required]
        [Url]
        public string CallbackBaseUrl { get; set; } = null!;
        // Exotel status callback endpoint
        [Required]
        [Url]
        public string StatusCallbackUrl { get; set; } = null!;
        [Range(5, 120)]
        public int RequestTimeoutSeconds { get; set; } = 30;
        [Range(10, 14400)]
        public int TimeLimitSeconds { get; set; } = 300;
        [Range(5, 120)]
        public int RingTimeoutSeconds { get; set; } = 30;
    }
}