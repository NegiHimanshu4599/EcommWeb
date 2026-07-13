using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Application.Interface.Provider;
using NotificationService.Domain.Configuration;
using NotificationService.Infrastructure.Constants;
using NotificationService.Infrastructure.Provider.Helpers;
using NotificationService.Infrastructure.Provider.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace NotificationService.Infrastructure.Provider
{
    public sealed class SmsSender : ISmsSender
    {
        private readonly HttpClient _httpClient;
        private readonly SmsSettings _settings;
        private readonly ILogger<SmsSender> _logger;
        public SmsSender(HttpClient httpClient, IOptions<SmsSettings> options, ILogger<SmsSender> logger)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authkey", _settings.AuthKey);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }
        public async Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            var requestId = Guid.NewGuid();
            ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);
            var normalizedNumber = PhoneNumberHelper.Normalize(phoneNumber, _settings.CountryCode);
            try
            {
                _logger.LogInformation("Sending SMS to {Phone}.", MaskPhone(normalizedNumber));
                var request = BuildRequest(normalizedNumber, message);
                using var response =await _httpClient.PostAsJsonAsync(SmsConstants.SmsEndpoint, request,cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var success = await response.Content.ReadFromJsonAsync<Msg91SmsResponse>(cancellationToken);
                    if (success is null)
                        throw new InvalidOperationException("Empty provider response.");
                    if (string.IsNullOrWhiteSpace(success.RequestId))
                        throw new InvalidOperationException("MSG91 returned no request id.");
                    _logger.LogInformation("SMS sent successfully. RequestId: {RequestId}", success?.RequestId);
                    return;
                }
                var error = await response.Content.ReadFromJsonAsync<Msg91ErrorResponse>(cancellationToken);
                _logger.LogError("MSG91 Error: {Message}", error?.Message);
                throw new InvalidOperationException(error?.Message ?? "SMS sending failed.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError( ex,"Unable to reach MSG91.");
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex,"MSG91 request timed out.");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,"Unable to deserialize MSG91 response.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Unexpected SMS error.");
                throw;
            }
        }
        private Msg91SmsRequest BuildRequest(string phoneNumber, string message)
        {
            return new Msg91SmsRequest
            {
                TemplateId = _settings.TemplateId,
                ShortUrl = "0",
                Recipients =
                [
                    new Msg91Recipient
                {
                    MobileNumber = phoneNumber,
                    Variable1 = message
                }
                ]
            };
        }
        private static string MaskPhone(string phone)
        {
            if (phone.Length <= 4)
                return phone;
            return $"{phone[..4]}******{phone[^2..]}";
        }
    }
}