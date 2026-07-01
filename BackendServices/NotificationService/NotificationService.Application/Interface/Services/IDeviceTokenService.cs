using NotificationService.Application.Dtos.DeviceToken;

namespace NotificationService.Application.Interface.Services
{
    public interface IDeviceTokenService
    {
        Task RegisterAsync(RegisterDeviceTokenDto dto);
        Task<IEnumerable<DeviceTokenDto>> GetByUserIdAsync(string userId);
        Task DeactivateAsync(int id);
        Task DeactivateByTokenAsync(string token);
    }
}