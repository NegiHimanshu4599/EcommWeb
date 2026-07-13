using System.ComponentModel.DataAnnotations;

namespace NotificationService.Domain.Configuration
{
    public class SmsSettings
    {
        public const string SectionName = "SmsSettings";

        [Required]
        public string BaseUrl { get; set; } = null!;
        [Required]
        public string AuthKey { get; set; } = null!;
        [Required]
        public string SenderId { get; set; } = null!;
        [Required]
        public string TemplateId { get; set; } = null!;
        [Required]
        public string Route { get; set; } = "4";
        [Required]
        public string CountryCode { get; set; } = "91";
        public int RequestTimeoutSeconds { get; set; } = 30;
    }
}