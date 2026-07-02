using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interface.Background
{
    public interface INotificationQueue
    {
        void Enqueue(Notification notification);
        Task<Notification> DequeueAsync(CancellationToken cancellationToken);
    }
}
