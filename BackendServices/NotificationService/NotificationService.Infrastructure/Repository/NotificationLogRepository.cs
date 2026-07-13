using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repository
{
    public class NotificationLogRepository : INotificationLogRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(NotificationLog notificationLog, CancellationToken cancellationToken = default)
        {
            await _context.NotificationLogs.AddAsync(notificationLog,cancellationToken);
        }
        public async Task<NotificationLog?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.NotificationLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
        }
        public async Task<IEnumerable<NotificationLog>> GetByNotificationIdAsync(int notificationId, CancellationToken cancellationToken = default)
        {
            return await _context.NotificationLogs.AsNoTracking().Where(x => x.NotificationId == notificationId).ToListAsync(cancellationToken);
        }
    }
}