using Microsoft.Extensions.Logging;
using NotificationService.Application.Interface.Provider;

namespace NotificationService.Application.Services.Provider
{
    public class PushNotificationSender : IPushNotificationSender
    {
        private readonly ILogger<PushNotificationSender> _logger;

        public PushNotificationSender(ILogger<PushNotificationSender> logger)
        {
            _logger = logger;
        }

        public async Task SendAsync(string deviceToken, string title, string body)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(deviceToken))
                throw new ArgumentException("Device token is required.", nameof(deviceToken));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Notification title is required.", nameof(title));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Notification body is required.", nameof(body));

            try
            {
                _logger.LogInformation(
                    "Sending push notification to device '{DeviceToken}' with title '{Title}'.",
                    deviceToken,
                    title);
                // ===========================================================
                // TODO:
                // Replace this section with Firebase Cloud Messaging (FCM)
                // implementation when Push Notifications are enabled.
                // ===========================================================
                await Task.CompletedTask;

                _logger.LogInformation(
                    "Push notification successfully sent to device '{DeviceToken}'.",
                    deviceToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send push notification to device '{DeviceToken}'.",
                    deviceToken);

                throw;
            }
        }
    }
}