using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Configuration
{
    public class OtpSettings
    {
        public int ExpiryMinutes { get; set; } = 10;
        public int CooldownSeconds { get; set; } = 60;
    }
}
