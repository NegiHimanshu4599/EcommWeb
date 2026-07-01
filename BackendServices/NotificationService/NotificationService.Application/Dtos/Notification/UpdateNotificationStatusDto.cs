using NotificationService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.Notification
{
    public class UpdateNotificationStatusDto
    {
        public NotificationStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
