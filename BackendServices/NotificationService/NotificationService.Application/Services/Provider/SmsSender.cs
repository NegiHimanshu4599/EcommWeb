using Microsoft.Extensions.Logging;
using NotificationService.Application.Interface.Provider;

namespace NotificationService.Application.Services.Provider
{
    public class SmsSender : ISmsSender
    {
        private readonly ILogger<SmsSender> _logger;
        public SmsSender(ILogger<SmsSender> logger)
        {
            _logger = logger;
        }
        public async Task SendAsync(string phoneNumber, string message)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("SMS message is required.", nameof(message));
            try
            {
                _logger.LogInformation("Sending SMS to '{PhoneNumber}'.", phoneNumber);
                // ==========================================================
                // TODO:
                // Replace this mock implementation with a real SMS provider.
                //
                // Examples:
                // - Twilio
                // - MSG91
                // - AWS SNS
                // - Azure Communication Services
                //
                // Example:
                // await _twilioClient.SendAsync(phoneNumber, message);
                // ==========================================================
                await Task.CompletedTask;
                _logger.LogInformation("SMS successfully sent to '{PhoneNumber}'.",phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to send SMS to '{PhoneNumber}'.",phoneNumber);
                throw;
            }
        }
    }
}