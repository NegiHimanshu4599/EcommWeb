using NotificationService.Domain.Enum;

namespace NotificationService.Application.Dtos.Otp
{
    public class ResendOtpDto
    {
        public string UserId { get; set; } = null!;
        public string Recipient { get; set; } = null!;
        public string? FullName { get; set; }
        public OtpType Type { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
