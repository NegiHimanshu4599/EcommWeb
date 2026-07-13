using NotificationService.Domain.Enum;

namespace NotificationService.Application.Dtos.Otp
{
    public class OtpDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Recipient { get; set; } = null!;
        public OtpType Type { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AttemptCount { get; set; }
        public DateTime ExpiryTime { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}
