using NotificationService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.NotificationLog
{
    public class NotificationLogDto
    {
        public int Id { get; set; }
        public string Provider { get; set; } = null!;
        public string? Response { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
