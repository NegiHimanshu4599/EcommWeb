using NotificationService.Domain.Enum;

namespace NotificationService.Domain.Entities
{
    public class OtpCode
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Recipient { get; set; } = null!;
        public string Code { get; set; } = null!;
        public OtpType Type { get; set; }
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; }
        public int AttemptCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}