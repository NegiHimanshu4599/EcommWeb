using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enum;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repository
{
    public class OtpRepository : IOtpRepository
    {
        private readonly ApplicationDbContext _context;
        public OtpRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(OtpCode otp, CancellationToken cancellationToken = default)
        {
            await _context.OtpCodes.AddAsync(otp,cancellationToken);
        }
        public void Delete(OtpCode otp)
        {
            _context.OtpCodes.Remove(otp);
        }
        public async Task<OtpCode?> GetActiveOtpAsync(string recipient, OtpType type, CancellationToken cancellationToken = default)
        {
            return await _context.OtpCodes.AsNoTracking().FirstOrDefaultAsync(x => x.Recipient == recipient && x.Type == type && !x.IsUsed && x.ExpiryTime > DateTime.UtcNow,cancellationToken);
        }
        public async Task<IEnumerable<OtpCode>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.OtpCodes.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        }
        public async Task<OtpCode?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.OtpCodes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id,cancellationToken);
        }
        public async Task<IEnumerable<OtpCode>> GetExpiredOtpsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.OtpCodes.AsNoTracking().Where(x => !x.IsUsed && x.ExpiryTime <= DateTime.UtcNow).ToListAsync(cancellationToken);      
        }
        public async Task<IEnumerable<OtpCode>> GetUserOtpsAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.OtpCodes.AsNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        }
        public void Update(OtpCode otp)
        {
            _context.OtpCodes.Update(otp);
        }
        public async Task<OtpCode?> GetLastOtpAsync(string recipient, OtpType type, CancellationToken cancellationToken = default)
        {
            return await _context.OtpCodes.AsNoTracking().Where(x => x.Recipient == recipient && x.Type == type).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(cancellationToken);
        }
    }
}