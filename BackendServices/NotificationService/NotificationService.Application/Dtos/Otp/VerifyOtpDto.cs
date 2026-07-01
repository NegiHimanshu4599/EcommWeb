using NotificationService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Dtos.Otp
{
    public class VerifyOtpDto
    {
        public string Recipient { get; set; } = null!;
        public string Code { get; set; } = null!;
        public OtpType Type { get; set; }
    }
}
