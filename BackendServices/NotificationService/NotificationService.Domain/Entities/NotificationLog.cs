using NotificationService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Entities
{
    public class NotificationLog
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;
        public string Provider { get; set; } = null!;
        public string? Response { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
