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
        Task<DeviceToken?> GetByIdAsync(int id ,CancellationToken cancellationToken=default);
        Task<DeviceToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<IEnumerable<DeviceToken>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task AddAsync(DeviceToken deviceToken, CancellationToken cancellationToken = default);
        void Update(DeviceToken deviceToken);
        void Delete(DeviceToken deviceToken);
    }
}