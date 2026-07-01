using NotificationService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DateTime ExpiryTime { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}
