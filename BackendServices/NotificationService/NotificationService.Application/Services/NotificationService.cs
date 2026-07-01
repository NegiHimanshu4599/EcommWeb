using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Dtos.Notification;
using NotificationService.Application.Interface.Provider;
using NotificationService.Application.Interface.Services;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enum;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IPushNotificationSender _pushSender;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper,ILogger<NotificationService> logger,IEmailSender emailSender, ISmsSender smsSender, IPushNotificationSender pushSender)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _pushSender = pushSender;
        }
        public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(dto.Recipient))
                throw new ArgumentException("Recipient is required.");
            if (string.IsNullOrWhiteSpace(dto.Body))
                throw new ArgumentException("Body is required.");
            if (dto.NotificationType == NotificationType.Email && string.IsNullOrWhiteSpace(dto.Subject))
                throw new ArgumentException("Subject is required for email notifications.");
            var notification = _mapper.Map<Notification>(dto);
            notification.Status = NotificationStatus.Pending;
            notification.RetryCount = 0;
            // If email template is provided, ensure it exists and use its subject/body?
            // Not specified – we'll just store the template reference.
            await _unitOfWork.NotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            // Process the notification asynchronously (or background) – for simplicity we do it inline.
            await ProcessNotificationAsync(notification);
            // Reload with logs and template
            notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notification.Id);
            return _mapper.Map<NotificationDto>(notification);
        }
        private async Task ProcessNotificationAsync(Notification notification)
        {
            try
            {
                notification.Status = NotificationStatus.Processing;
                await _unitOfWork.NotificationRepository.UpdateAsync(notification);
                await _unitOfWork.SaveChangesAsync();
                string provider = null;
                bool success = false;
                string response = null;
                switch (notification.NotificationType)
                {
                    case NotificationType.Email:
                        provider = "SendGrid";
                        await _emailSender.SendAsync(notification.Recipient, notification.Subject, notification.Body);
                        break;
                    case NotificationType.SMS:
                        provider = "Twilio";
                        await _smsSender.SendAsync(notification.Recipient, notification.Body);
                        break;
                    case NotificationType.Push:
                        var tokens = await _unitOfWork.DeviceTokenRepository.GetByUserIdAsync(notification.UserId);
                        if (!tokens.Any())
                            throw new InvalidOperationException("No active device tokens found for user.");
                        foreach (var token in tokens)
                        {
                            await _pushSender.SendAsync(token.Token, notification.Subject ?? "Notification", notification.Body);
                        }
                        provider = "Firebase";
                        break;
                    case NotificationType.InApp:
                        provider = "InAppStorage";
                        break;
                    default:
                        throw new NotSupportedException("Notification type not supported.");
                }
                success = true;
                response = "Success";
                notification.Status = NotificationStatus.Sent;
                notification.SentAt = DateTime.UtcNow;
                notification.ErrorMessage = null;
                await LogNotificationAttempt(notification.Id, provider, response, NotificationStatus.Sent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification {NotificationId}", notification.Id);
                notification.Status = NotificationStatus.Failed;
                notification.ErrorMessage = ex.Message;
                notification.RetryCount++;
                await LogNotificationAttempt(notification.Id, "Unknown", ex.Message, NotificationStatus.Failed);
            }
            finally
            {
                await _unitOfWork.NotificationRepository.UpdateAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }
        }
        private async Task LogNotificationAttempt(int notificationId, string provider, string response, NotificationStatus status)
        {
            var log = new NotificationLog
            {
                NotificationId = notificationId,
                Provider = provider,
                Response = response?.Substring(0, Math.Min(response.Length, 4000)),
                Status = status,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.NotificationLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<NotificationDto>> GetAllAsync()
        {
            var notifications = await _unitOfWork.NotificationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }
        public async Task<NotificationDto?> GetByIdAsync(int id)
        {
            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(id);
            return notification == null ? null : _mapper.Map<NotificationDto>(notification);
        }
        public async Task UpdateStatusAsync(int id, UpdateNotificationStatusDto dto)
        {
            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(id);
            if (notification == null)
                throw new ArgumentException($"Notification with id {id} not found.");
            notification.Status = dto.Status;
            notification.ErrorMessage = dto.ErrorMessage;
            if (dto.Status == NotificationStatus.Sent)
                notification.SentAt = DateTime.UtcNow;
            await _unitOfWork.NotificationRepository.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(id);
            if (notification != null)
            {
                await _unitOfWork.NotificationRepository.DeleteAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}