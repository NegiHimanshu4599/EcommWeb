using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _context.Notifications.AddAsync(notification,cancellationToken);
        }
        public void Delete(Notification notification)
        {
            _context.Notifications.Remove(notification);
        }
        public async Task<IEnumerable<Notification>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.AsNoTracking().Include(x => x.NotificationLogs).Include(x => x.EmailTemplate).ToListAsync(cancellationToken);
        }
        public async Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.Include(x => x.NotificationLogs).Include(x => x.EmailTemplate).FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
        }
        public void Update(Notification notification)
        {
            _context.Notifications.Update(notification);
        }
    }
}