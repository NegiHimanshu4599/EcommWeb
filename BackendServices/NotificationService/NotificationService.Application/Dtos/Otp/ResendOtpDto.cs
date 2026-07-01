using NotificationService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
