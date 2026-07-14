using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interface;
using AutoMapper;

namespace AuthService.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMapper _mapper;
        public UserProfileService(IUserProfileRepository userProfileRepository , IMapper mapper)
        {
            _userProfileRepository = userProfileRepository;
            _mapper = mapper;
        }
        public async Task<UserProfileDto> GetProfile(string userId)
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException("Profile Not Found");
            return _mapper.Map<UserProfileDto>(profile);
        } 
        public async Task<UserProfileDto> UpdateProfile(string userId, UpdateUserProfileDto dto)
        {
            var profile =  await _userProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                throw new KeyNotFoundException("Profile not found");
            profile.FullName = dto.FullName;
            profile.PrimaryPhoneNumber = dto.PrimaryPhoneNumber;
            profile.UpdatedAt = DateTime.UtcNow;
            _userProfileRepository.Update(profile);
            await _userProfileRepository.SaveAsync();
            return _mapper.Map<UserProfileDto>(profile);
        }
    }
}