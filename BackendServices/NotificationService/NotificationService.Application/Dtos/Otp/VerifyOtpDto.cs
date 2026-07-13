using NotificationService.Domain.Enum;

namespace NotificationService.Application.Dtos.Otp
{
    public class VerifyOtpDto
    {
        public string Recipient { get; set; } = null!;
        public string Code { get; set; } = null!;
        public OtpType Type { get; set; }
    }
}
