using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        public INotificationRepository NotificationRepository { get; }
        public INotificationLogRepository NotificationLogRepository { get; }
        public IDeviceTokenRepository DeviceTokenRepository { get; }
        public IEmailTemplateRepository EmailTemplateRepository { get; }
        public IOtpRepository OtpRepository { get; }
        Task SaveChangesAsync(CancellationToken cancellationToken =default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken=default);
    }
}