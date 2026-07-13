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
        public async Task AddAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default)
        {
            await _context.DeviceTokens.AddAsync(deviceToken, cancellationToken);
        }
        public void Delete(DeviceToken deviceToken)
        {
            _context.DeviceTokens.Remove(deviceToken);
        }
        public async Task<DeviceToken?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id , cancellationToken);
        }
        public async Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == token,cancellationToken);
        }
        public async Task<IEnumerable<DeviceToken>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.DeviceTokens.AsNoTracking().Where(x => x.UserId == userId && x.IsActive ).ToListAsync(cancellationToken);
        }
        public void Update(DeviceToken deviceToken)
        {
           _context.DeviceTokens.Update(deviceToken);
        }
    }
}