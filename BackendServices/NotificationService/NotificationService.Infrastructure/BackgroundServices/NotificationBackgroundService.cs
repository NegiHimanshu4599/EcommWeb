using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Interface.Background;
using NotificationService.Application.Interface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.BackgroundServices
{
    public class NotificationBackgroundService: BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly INotificationQueue _queue;
        private readonly ILogger<NotificationBackgroundService> _logger;
        public NotificationBackgroundService(IServiceProvider services , INotificationQueue queue, ILogger<NotificationBackgroundService> logger)
        {
            _logger = logger;
            _queue = queue;
            _services = services;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var notification = await _queue.DequeueAsync(stoppingToken);
                try
                {
                    using var scope = _services.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    await service.ProcessNotificationAsync(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing notification {Id}", notification.Id);
                }
            }
        }
    }
}
