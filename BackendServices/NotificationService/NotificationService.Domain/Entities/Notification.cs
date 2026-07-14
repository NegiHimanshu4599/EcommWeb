using NotificationService.Domain.Enum;

namespace NotificationService.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Recipient { get; set; } = null!;
        public NotificationType NotificationType { get; set; }
        public string? Subject { get; set; }
        public string Body { get; set; } = null!;
        public NotificationStatus Status { get; set; }
        public NotificationPriority Priority { get; set; }
        public int RetryCount { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public int? EmailTemplateId { get; set; }
        public EmailTemplate? EmailTemplate { get; set; }
        public string RequestId { get; set; } = Guid.NewGuid().ToString("N");
        public ICollection<NotificationLog> NotificationLogs { get; set; }  = new List<NotificationLog>();
    }
}