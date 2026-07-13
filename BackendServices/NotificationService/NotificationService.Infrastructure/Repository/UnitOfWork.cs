using Microsoft.EntityFrameworkCore.Storage;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            NotificationRepository = new NotificationRepository(context);
            NotificationLogRepository = new NotificationLogRepository(context);
            DeviceTokenRepository = new DeviceTokenRepository(context);
            EmailTemplateRepository = new EmailTemplateRepository(context);
            OtpRepository = new OtpRepository(context);
        }
        public IOtpRepository OtpRepository { private set; get; }
        public INotificationRepository NotificationRepository { private set; get; }
        public INotificationLogRepository NotificationLogRepository { private set; get; }
        public IDeviceTokenRepository DeviceTokenRepository { private set; get; }
        public IEmailTemplateRepository EmailTemplateRepository { private set; get; }
        public async Task SaveChangesAsync(CancellationToken cancellationToken=default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken=default)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }
    }
}