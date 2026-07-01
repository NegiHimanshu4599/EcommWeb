using NotificationService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.Notification
{
    public class CreateNotificationDto
    {
        public string UserId { get; set; } = null!;
        public string Recipient { get; set; } = null!;
        public NotificationType NotificationType { get; set; }
        public string? Subject { get; set; }
        public string Body { get; set; } = null!;
        public NotificationPriority Priority { get; set; }
        public int? EmailTemplateId { get; set; }
    }
}
