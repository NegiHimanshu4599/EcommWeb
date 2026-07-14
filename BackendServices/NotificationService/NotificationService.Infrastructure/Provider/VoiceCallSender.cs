using Microsoft.Extensions.Logging;
using NotificationService.Application.Interface.Provider;

namespace NotificationService.Infrastructure.Provider
{
    public class VoiceCallSender : IVoiceCallSender
    {
        private readonly ILogger<VoiceCallSender> _logger;
        public VoiceCallSender(ILogger<VoiceCallSender> logger)
        {
            _logger = logger;
        }
        public async Task SendVoiceMessageAsync(string phoneNumber, string message ,CancellationToken cancellationToken=default)
        {
            ValidatePhoneNumber(phoneNumber);
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Voice message is required.", nameof(message));
            try
            {
                _logger.LogInformation("Sending voice message to '{PhoneNumber}'.", phoneNumber);
                await Task.CompletedTask;
                _logger.LogInformation("Voice message successfully sent to '{PhoneNumber}'.", phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send voice message to '{PhoneNumber}'.", phoneNumber);
                throw;
            }
        }
        public async Task SendOtpCallAsync(string phoneNumber, string otp, CancellationToken cancellationToken = default)
        {
            ValidatePhoneNumber(phoneNumber);
            if (string.IsNullOrWhiteSpace(otp))
                throw new ArgumentException("OTP is required.", nameof(otp));
            try
            {
                _logger.LogInformation("Sending OTP voice call to '{PhoneNumber}'.", phoneNumber);
                await Task.CompletedTask;
                _logger.LogInformation("OTP voice call successfully completed to '{PhoneNumber}'.", phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP voice call to '{PhoneNumber}'.", phoneNumber);
                throw;
            }
        }
        private static void ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));
        }
    }
}