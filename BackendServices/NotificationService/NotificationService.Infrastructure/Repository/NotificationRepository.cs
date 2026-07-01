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
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }
        public Task DeleteAsync(Notification notification)
        {
            _context.Notifications.Remove(notification);
            return Task.CompletedTask;
        }
        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _context.Notifications.AsNoTracking().Include(x => x.NotificationLogs).Include(x => x.EmailTemplate).ToListAsync();
        }
        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications.AsNoTracking().Include(x => x.NotificationLogs).Include(x => x.EmailTemplate).FirstOrDefaultAsync(x => x.Id == id);
        }
        public Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            return Task.CompletedTask;
        }
    }
}