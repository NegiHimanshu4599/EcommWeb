using AutoMapper;
using NotificationService.Application.Dtos.DeviceToken;
using NotificationService.Application.Interface.Services;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Services
{
    public class DeviceTokenService : IDeviceTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DeviceTokenService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task RegisterAsync(RegisterDeviceTokenDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.UserId))
                throw new ArgumentException("UserId is required.", nameof(dto.UserId));
            if (string.IsNullOrWhiteSpace(dto.Token))
                throw new ArgumentException("Device token is required.", nameof(dto.Token));
            if (string.IsNullOrWhiteSpace(dto.Platform))
                throw new ArgumentException("Platform is required.", nameof(dto.Platform));
            var existingToken = await _unitOfWork.DeviceTokenRepository.GetByTokenAsync(dto.Token);
            if (existingToken != null)
            {
                existingToken.UserId = dto.UserId;
                existingToken.Platform = dto.Platform;
                existingToken.IsActive = true;
                existingToken.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.DeviceTokenRepository.UpdateAsync(existingToken);
            }
            else
            {
                var deviceToken = _mapper.Map<DeviceToken>(dto);
                deviceToken.IsActive = true;
                deviceToken.CreatedAt = DateTime.UtcNow;
                await _unitOfWork.DeviceTokenRepository.AddAsync(deviceToken);
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<IEnumerable<DeviceTokenDto>> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.", nameof(userId));
            var deviceTokens = await _unitOfWork.DeviceTokenRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<DeviceTokenDto>>(deviceTokens);
        }
        public async Task DeactivateAsync(int id)
        {
            var deviceToken = await _unitOfWork.DeviceTokenRepository.GetByIdAsync(id);
            if (deviceToken == null)
                throw new KeyNotFoundException("Device token not found.");
            if (!deviceToken.IsActive)
                return;
            deviceToken.IsActive = false;
            deviceToken.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.DeviceTokenRepository.UpdateAsync(deviceToken);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeactivateByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Device token is required.", nameof(token));
            var deviceToken = await _unitOfWork.DeviceTokenRepository.GetByTokenAsync(token);
            if (deviceToken == null)
                throw new KeyNotFoundException("Device token not found.");
            if (!deviceToken.IsActive)
                return;
            deviceToken.IsActive = false;
            deviceToken.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.DeviceTokenRepository.UpdateAsync(deviceToken);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}