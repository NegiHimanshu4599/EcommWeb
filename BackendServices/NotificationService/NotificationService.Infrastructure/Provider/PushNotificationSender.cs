using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Dtos.PushNotification;
using NotificationService.Application.Interface.Provider;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Provider
{
    public sealed class PushNotificationSender : IPushNotificationSender
    {
        private readonly ILogger<PushNotificationSender> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public PushNotificationSender(ILogger<PushNotificationSender> logger, IUnitOfWork unitofwork)
        {
            _logger = logger;
            _unitOfWork = unitofwork;
        }
        public async Task SendAsync(PushNotificationRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (string.IsNullOrWhiteSpace(request.DeviceToken))
                throw new ArgumentException("DeviceToken is required.", nameof(request.DeviceToken));
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required.", nameof(request.Title));
            if (string.IsNullOrWhiteSpace(request.Body))
                throw new ArgumentException("Body is required.", nameof(request.Body));
            try
            {
                var maskedToken = request.DeviceToken.Length > 8 ? $"{request.DeviceToken[..8]}...": request.DeviceToken;
                _logger.LogInformation("Sending push notification to device {DeviceToken}", maskedToken);
                var message = BuildMessage(request);
                var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
                _logger.LogInformation("Push notification sent successfully. MessageId: {MessageId}", messageId);
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, "Firebase failed while sending notification.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending push notification.");
                throw;
            }
        }
        public async Task SendMulticastAsync(MulticastPushNotificationRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (!request.DeviceTokens.Any())
                throw new ArgumentException("At least one device token is required.");
            try
            {
                _logger.LogInformation("Sending multicast notification to {Count} devices.", request.DeviceTokens.Count());
                var message = BuildMulticastMessage(request);
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message, cancellationToken);
                _logger.LogInformation("Firebase Success: {Success} Failed: {Failed}", response.SuccessCount, response.FailureCount);
                await HandleFailedTokensAsync(request.DeviceTokens.ToList(), response);
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, "Firebase multicast failed.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected multicast error.");
                throw;
            }
        }
        private Message BuildMessage(PushNotificationRequest request)
        {
            return new Message
            {
                Token = request.DeviceToken,
                Notification = new Notification
                {
                    Title = request.Title,
                    Body = request.Body,
                    ImageUrl = request.ImageUrl
                },
                Data = request.Data,
                Android = new AndroidConfig
                {
                    Priority = request.HighPriority ? Priority.High : Priority.Normal,
                    Notification = new AndroidNotification
                    {
                        ClickAction = request.ClickAction
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                },
                Webpush = new WebpushConfig
                {
                    Notification = new WebpushNotification
                    {
                        Title = request.Title,
                        Body = request.Body,
                        Icon = request.ImageUrl
                    },
                    FcmOptions = new WebpushFcmOptions
                    {
                        Link = request.ClickAction
                    }
                }
            };
        }
        private static MulticastMessage BuildMulticastMessage(MulticastPushNotificationRequest request)
        {
            return new MulticastMessage
            {
                Tokens = request.DeviceTokens.ToList(),
                Notification = new Notification
                {
                    Title = request.Title,
                    Body = request.Body,
                    ImageUrl = request.ImageUrl
                },
                Data = request.Data,
                Android = new AndroidConfig
                {
                    Priority = request.HighPriority
                        ? Priority.High
                        : Priority.Normal,

                    Notification = new AndroidNotification
                    {
                        ClickAction = request.ClickAction
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Sound = "default"
                    }
                },
                Webpush = new WebpushConfig
                {
                    Notification = new WebpushNotification
                    {
                        Title = request.Title,
                        Body = request.Body,
                        Icon = request.ImageUrl
                    },
                    FcmOptions = new WebpushFcmOptions
                    {
                        Link = request.ClickAction
                    }
                }
            };
        }
        private async Task HandleFailedTokensAsync(IReadOnlyList<string> deviceTokens, BatchResponse response)
        {
            bool hasChanges = false;
            for (var i = 0; i < response.Responses.Count; i++)
            {
                var result = response.Responses[i];
                if (result.IsSuccess)
                    continue;
                var token = deviceTokens[i];
                _logger.LogWarning("Push notification failed for token {Token}. ErrorCode: {ErrorCode}", token, result.Exception?.MessagingErrorCode);
                switch (result.Exception?.MessagingErrorCode)
                {
                    case MessagingErrorCode.Unregistered:
                        _logger.LogInformation("Device token {Token} is unregistered.", token);
                        var device = await _unitOfWork.DeviceTokenRepository.GetByTokenAsync(token);
                        if (device is not null)
                        {
                            device.IsActive = false;
                            device.UpdatedAt = DateTime.UtcNow;
                            _unitOfWork.DeviceTokenRepository.Update(device);
                            hasChanges = true;
                        }
                        break;
                    case MessagingErrorCode.InvalidArgument:
                        _logger.LogInformation("Invalid Firebase token {Token}.", token);
                        break;
                    default:
                        _logger.LogWarning("Firebase returned {ErrorCode} for token {Token}.", result.Exception?.MessagingErrorCode, token);
                        break;
                }
            }
            if (hasChanges)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}