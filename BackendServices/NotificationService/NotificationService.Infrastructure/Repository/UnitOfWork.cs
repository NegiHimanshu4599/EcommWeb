using Microsoft.EntityFrameworkCore.Storage;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }
    }
}