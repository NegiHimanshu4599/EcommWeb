using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Enum
{
    public enum NotificationStatus
    {
        Pending,
        Processing,
        Sent,
        Failed,
        Cancelled
    }
}
