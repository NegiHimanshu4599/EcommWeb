using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Notification>> GetAllAsync( CancellationToken cancellationToken = default);
        Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
        void  Update(Notification notification);
        void Delete(Notification notification);
    }
}
