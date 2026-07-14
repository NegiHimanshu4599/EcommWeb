using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interface.Background
{
    public interface INotificationQueue
    {
        Task EnqueueAsync(Notification notification, CancellationToken cancellationToken = default);
        Task<Notification> DequeueAsync(CancellationToken cancellationToken);
    }
}
