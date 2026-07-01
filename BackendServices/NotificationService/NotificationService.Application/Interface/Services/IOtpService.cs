using NotificationService.Application.Dtos.Otp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Application.Interface.Services
{
    public interface IOtpService
    {
        Task<OtpResultDto> GenerateAsync(GenerateOtpDto dto, CancellationToken cancellationToken = default);
        Task<OtpResultDto> VerifyAsync(VerifyOtpDto dto, CancellationToken cancellationToken = default);
        Task<OtpResultDto> ResendAsync(ResendOtpDto dto, CancellationToken cancellationToken = default);
        Task<int> CleanupExpiredOtpsAsync(CancellationToken cancellationToken = default);
    }
}
