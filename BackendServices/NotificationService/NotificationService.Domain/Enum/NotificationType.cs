using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Enum
{
    public enum NotificationType
    {
        Email =0,
        SMS = 1,
        Push=2,
        InApp = 3,
        VoiceCall = 4
    }
}
