//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using NotificationService.Application.Interface.Services;
//using NotificationService.Domain.Configuration;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NotificationService.Infrastructure.BackgroundServices
//{
//    public class ExpiredOtpCleanupService : BackgroundService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        private readonly ILogger<ExpiredOtpCleanupService> _logger;
//        private readonly OtpSettings _settings;
//        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // run every 5 minutes
//        public ExpiredOtpCleanupService(IServiceProvider serviceProvider, ILogger<ExpiredOtpCleanupService> logger, IOptions<OtpSettings> settings)
//        {
//            _serviceProvider = serviceProvider;
//            _logger = logger;
//            _settings = settings.Value;
//        }
//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                try
//                {
//                    using var scope = _serviceProvider.CreateScope();
//                    var otpService = scope.ServiceProvider.GetRequiredService<IOtpService>();
//                    var cleaned = await otpService.CleanupExpiredOtpsAsync(stoppingToken);
//                    if (cleaned > 0)
//                        _logger.LogInformation("Cleaned {Cleaned} expired OTPs.", cleaned);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error occurred while cleaning expired OTPs.");
//                }
//                await Task.Delay(_interval, stoppingToken);
//            }
//        }
//    }
//}