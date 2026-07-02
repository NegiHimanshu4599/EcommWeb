using NotificationService.Application.Interface.Background;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.BackgroundServices
{
    public class NotificationQueue:INotificationQueue
    {
        private readonly Channel<Notification> _channel = Channel.CreateUnbounded<Notification>();
        public void Enqueue(Notification notification) => _channel.Writer.TryWrite(notification);
        public async Task<Notification> DequeueAsync(CancellationToken cancellationToken)
            => await _channel.Reader.ReadAsync(cancellationToken);
    }
}
