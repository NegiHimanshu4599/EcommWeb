using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Repository
{
    public class NotificationLogRepository : INotificationLogRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(NotificationLog notificationLog)
        {
            await _context.NotificationLogs.AddAsync(notificationLog);
        }
        public async Task<NotificationLog?> GetByIdAsync(int id)
        {
            return await _context.NotificationLogs.AsNoTracking().FirstOrDefaultAsync(x=>x.Id ==id);
        }
        public async Task<IEnumerable<NotificationLog>> GetByNotificationIdAsync(int notificationId)
        {
            return await _context.NotificationLogs.AsNoTracking().Where(x => x.NotificationId == notificationId).ToListAsync();
        }
    }
}
