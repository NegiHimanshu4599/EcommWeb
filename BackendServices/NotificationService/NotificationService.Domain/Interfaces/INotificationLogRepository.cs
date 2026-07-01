using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Interfaces
{
    public interface INotificationLogRepository
    {
        Task<NotificationLog?> GetByIdAsync(int id);
        Task<IEnumerable<NotificationLog>> GetByNotificationIdAsync(int notificationId);
        Task AddAsync(NotificationLog notificationLog);
    }
}