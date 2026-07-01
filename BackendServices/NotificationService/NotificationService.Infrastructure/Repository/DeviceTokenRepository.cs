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
    public class DeviceTokenRepository : IDeviceTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public DeviceTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(DeviceToken deviceToken)
        {
            await _context.DeviceTokens.AddAsync(deviceToken);
        }
        public Task DeleteAsync(DeviceToken deviceToken)
        {
            _context.DeviceTokens.Remove(deviceToken);
            return Task.CompletedTask;
        }
        public async Task<DeviceToken?> GetByTokenAsync(string token)
        {
            return await _context.DeviceTokens.FirstOrDefaultAsync(x => x.Token == token);
        }
        public async Task<IEnumerable<DeviceToken>> GetByUserIdAsync(string userId)
        {
           return await _context.DeviceTokens.AsNoTracking().Where(x => x.UserId == userId && x.IsActive).ToListAsync();
        }
        public  Task UpdateAsync(DeviceToken deviceToken)
        {
             _context.DeviceTokens.Update(deviceToken);
            return  Task.CompletedTask;
        }
        public async Task<DeviceToken?> GetByIdAsync(int id)
        {
            return await _context.DeviceTokens.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}