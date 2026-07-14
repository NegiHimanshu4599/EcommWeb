using System.ComponentModel.DataAnnotations;

namespace NotificationService.Domain.Configuration
{
    public class OtpSettings
    {
        [Range(1, 60)]
        public int ExpiryMinutes { get; set; }
        [Range(0, 600)]
        public int CooldownSeconds { get; set; }
        [Range(1, 20)]
        public int MaxVerificationAttempts { get; set; }
        [Range(1, 10)]
        public int MaxResendAttempts { get; set; }
    }
}
