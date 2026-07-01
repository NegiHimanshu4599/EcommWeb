using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Application.Dtos.Otp;
using NotificationService.Application.Interface.Provider;
using NotificationService.Application.Interface.Services;
using NotificationService.Domain.Configuration;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enum;
using NotificationService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OtpService> _logger;
        private readonly IEmailTemplateRenderer _templateRenderer;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IVoiceCallSender _voiceCallSender;
        private readonly IMapper _mapper;
        private readonly OtpSettings _settings;
        public OtpService( IUnitOfWork unitOfWork, IOptions<OtpSettings> options,ILogger<OtpService> logger, IEmailTemplateRenderer templateRenderer,  IEmailSender emailSender, ISmsSender smsSender,IVoiceCallSender voiceCallSender, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _settings = options.Value;
            _logger = logger;
            _templateRenderer = templateRenderer;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _voiceCallSender = voiceCallSender;
            _mapper = mapper;
        }
        private static string GenerateOtpCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }
        private static Dictionary<string, string> BuildPlaceholders(string fullName, string otp)
        {
            return new Dictionary<string, string>
            {
                { "FullName", fullName },
                { "OTP", otp },
                { "Minutes", "10" } // We'll replace with configured value later if needed
            };
        }
        private async Task<OtpCode> CreateOtpAsync(GenerateOtpDto dto, string otp, CancellationToken cancellationToken = default)
        {
            var entity = new OtpCode
            {
                UserId = dto.UserId,
                Recipient = dto.Recipient,
                Code = otp,
                Type = dto.Type,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes) // use configured expiry
            };
            await _unitOfWork.OtpRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return entity;
        }
        private async Task SendEmailOtpAsync(string recipient, string fullName, string otp, CancellationToken cancellationToken = default)
        {
            var placeholders = BuildPlaceholders(fullName, otp);
            var rendered = await _templateRenderer.RenderAsync("EmailVerification", placeholders);
            await _emailSender.SendAsync(recipient, rendered.Subject, rendered.HtmlBody);
        }
        private async Task DeactivateExistingOtpAsync(string recipient, OtpType type, CancellationToken cancellationToken = default)
        {
            var existing = await _unitOfWork.OtpRepository.GetActiveOtpAsync(recipient, type, cancellationToken);
            if (existing == null)
                return;
            existing.IsUsed = true;
            existing.VerifiedAt = DateTime.UtcNow;
            await _unitOfWork.OtpRepository.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        private async Task SendSmsOtpAsync(string recipient, string otp, CancellationToken cancellationToken = default)
        {
            var message = $"Your verification code is {otp}. It expires in {_settings.ExpiryMinutes} minutes.";
            await _smsSender.SendAsync(recipient, message);
        }
        private async Task SendVoiceOtpAsync(string recipient, string otp, CancellationToken cancellationToken = default)
        {
            await _voiceCallSender.SendOtpCallAsync(recipient, otp);
        }
        private async Task SendOtpAsync(NotificationType notificationType, string recipient, string fullName, string otp, CancellationToken cancellationToken = default)
        {
            switch (notificationType)
            {
                case NotificationType.Email:
                    await SendEmailOtpAsync(recipient, fullName, otp, cancellationToken);
                    break;
                case NotificationType.SMS:
                    await SendSmsOtpAsync(recipient, otp, cancellationToken);
                    break;
                case NotificationType.VoiceCall:
                    await SendVoiceOtpAsync(recipient, otp, cancellationToken);
                    break;
                default:
                    throw new NotSupportedException($"Notification type '{notificationType}' is not supported for OTP.");
            }
        }
        public async Task<OtpResultDto> GenerateAsync(GenerateOtpDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.UserId))
                throw new ArgumentException("UserId is required.", nameof(dto.UserId));
            if (string.IsNullOrWhiteSpace(dto.Recipient))
                throw new ArgumentException("Recipient is required.", nameof(dto.Recipient));
            _logger.LogInformation("Generating OTP for '{Recipient}'.", dto.Recipient);
            // Deactivate any existing active OTPs for this recipient/type
            await DeactivateExistingOtpAsync(dto.Recipient, dto.Type, cancellationToken);
            var otp = GenerateOtpCode();
            await CreateOtpAsync(dto, otp, cancellationToken);
            await SendOtpAsync(dto.NotificationType, dto.Recipient, dto.FullName ?? "User", otp, cancellationToken);
            _logger.LogInformation("OTP generated successfully for '{Recipient}'.", dto.Recipient);
            return new OtpResultDto
            {
                Success = true,
                Message = "OTP generated successfully."
            };
        }
        public async Task<OtpResultDto> VerifyAsync(VerifyOtpDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Recipient))
                throw new ArgumentException("Recipient is required.", nameof(dto.Recipient));
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("OTP is required.", nameof(dto.Code));
            var otp = await _unitOfWork.OtpRepository.GetActiveOtpAsync(dto.Recipient, dto.Type, cancellationToken);
            if (otp == null)
            {
                return new OtpResultDto
                {
                    Success = false,
                    Message = "OTP not found or already expired."
                };
            }
            if (otp.IsUsed)
            {
                return new OtpResultDto
                {
                    Success = false,
                    Message = "OTP has already been used."
                };
            }
            if (otp.ExpiryTime <= DateTime.UtcNow)
            {
                return new OtpResultDto
                {
                    Success = false,
                    Message = "OTP has expired."
                };
            }
            if (otp.Code != dto.Code)
            {
                return new OtpResultDto
                {
                    Success = false,
                    Message = "Invalid OTP."
                };
            }
            otp.IsUsed = true;
            otp.VerifiedAt = DateTime.UtcNow;
            await _unitOfWork.OtpRepository.UpdateAsync(otp);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("OTP verified successfully for '{Recipient}'.", dto.Recipient);
            return new OtpResultDto
            {
                Success = true,
                Message = "OTP verified successfully."
            };
        }
        public async Task<OtpResultDto> ResendAsync(ResendOtpDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.UserId))
                throw new ArgumentException("UserId is required.", nameof(dto.UserId));
            if (string.IsNullOrWhiteSpace(dto.Recipient))
                throw new ArgumentException("Recipient is required.", nameof(dto.Recipient));
            // Check if resend is allowed (cooldown)
            var canResend = await CanResendAsync(dto.Recipient, dto.Type, cancellationToken);
            if (!canResend)
            {
                _logger.LogWarning("Resend requested too soon for '{Recipient}' ({Type}).", dto.Recipient, dto.Type);
                return new OtpResultDto
                {
                    Success = false,
                    Message = $"Please wait {_settings.CooldownSeconds} seconds before requesting a new OTP."
                };
            }
            // Deactivate any existing active OTPs for this recipient/type
            await DeactivateExistingOtpAsync(dto.Recipient, dto.Type, cancellationToken);
            // Generate new OTP
            var otp = GenerateOtpCode();
            var entity = await CreateOtpAsync(new GenerateOtpDto
            {
                UserId = dto.UserId,
                Recipient = dto.Recipient,
                FullName = dto.FullName,
                Type = dto.Type,
                NotificationType = dto.NotificationType
            }, otp, cancellationToken);
            // Send the OTP
            await SendOtpAsync(dto.NotificationType, dto.Recipient, dto.FullName ?? "User", otp, cancellationToken);
            _logger.LogInformation("OTP resent successfully for '{Recipient}'.", dto.Recipient);
            return new OtpResultDto
            {
                Success = true,
                Message = "OTP resent successfully."
            };
        }
        /// <summary>
        /// Checks if a new OTP can be sent based on cooldown period.
        /// </summary>
        private async Task<bool> CanResendAsync(string recipient, OtpType type, CancellationToken cancellationToken = default)
        {
            var lastOtp = await _unitOfWork.OtpRepository.GetActiveOtpAsync(recipient, type, cancellationToken);
            if (lastOtp == null)
                return true; // No existing OTP, so resend is allowed
            // If the last OTP was created within the cooldown period, block resend
            var timeSinceCreation = DateTime.UtcNow - lastOtp.CreatedAt;
            return timeSinceCreation.TotalSeconds >= _settings.CooldownSeconds;
        }
        /// <summary>
        /// Cleans up expired OTPs by marking them as used (or soft-deleting).
        /// </summary>
        /// <returns>Number of OTPs cleaned.</returns>
        public async Task<int> CleanupExpiredOtpsAsync(CancellationToken cancellationToken = default)
        {
            var expiredOtps = await _unitOfWork.OtpRepository.GetExpiredOtpsAsync();
            var count = 0;
            foreach (var otp in expiredOtps)
            {
                // Mark as used so they won't be considered active
                otp.IsUsed = true;
                // Optionally set VerifiedAt to null or leave as is
                await _unitOfWork.OtpRepository.UpdateAsync(otp);
                count++;
            }
            if (count > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Cleaned up {Count} expired OTPs.", count);
            }
            return count;
        }
       
    }
}
