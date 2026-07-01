using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces
{
    public interface IDeviceTokenRepository
    {
        Task<DeviceToken?> GetByIdAsync(int id);
        Task<DeviceToken?> GetByTokenAsync(string token);
        Task<IEnumerable<DeviceToken>> GetByUserIdAsync(string userId);
        Task AddAsync(DeviceToken deviceToken);
        Task UpdateAsync(DeviceToken deviceToken);
        Task DeleteAsync(DeviceToken deviceToken);
    }
}