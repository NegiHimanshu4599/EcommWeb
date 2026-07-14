using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces
{
    public interface INotificationLogRepository
    {
        Task<NotificationLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<NotificationLog>> GetByNotificationIdAsync(int notificationId, CancellationToken cancellationToken = default);
        Task AddAsync(NotificationLog notificationLog, CancellationToken cancellationToken = default);
    }
}