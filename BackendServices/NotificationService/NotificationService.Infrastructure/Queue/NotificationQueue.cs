using NotificationService.Application.Interface.Background;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Queue
{
    public class NotificationQueue:INotificationQueue
    {
        private readonly Channel<Notification> _channel = Channel.CreateBounded<Notification>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
        public async Task<Notification> DequeueAsync(CancellationToken cancellationToken)
            => await _channel.Reader.ReadAsync(cancellationToken);
        public async Task EnqueueAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _channel.Writer.WriteAsync(notification, cancellationToken);
        }
    }
}