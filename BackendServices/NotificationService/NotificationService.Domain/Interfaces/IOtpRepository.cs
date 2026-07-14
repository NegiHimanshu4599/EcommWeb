using NotificationService.Domain.Entities;
using NotificationService.Domain.Enum;

namespace NotificationService.Domain.Interfaces
{
    public interface IOtpRepository
    {
        Task<OtpCode?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<OtpCode>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<OtpCode?> GetActiveOtpAsync(string recipient, OtpType type, CancellationToken cancellationToken = default);
        Task<IEnumerable<OtpCode>> GetUserOtpsAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<OtpCode>> GetExpiredOtpsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(OtpCode otp, CancellationToken cancellationToken = default);
        void Update(OtpCode otp);
        void Delete(OtpCode otp); 
        Task<OtpCode?> GetLastOtpAsync(string recipient, OtpType type, CancellationToken cancellationToken = default);
    }
}
