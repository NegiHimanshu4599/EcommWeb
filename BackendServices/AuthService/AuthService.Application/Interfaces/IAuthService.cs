using AuthService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterUserDto Dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto Dto);
        Task<UserProfileDto> GetProfileAsync(string userId);
        Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateUserDto dto);
        Task<bool> DeactivateUserAsync(string userId);
        Task LogoutAsync(LogoutDto dto);
        Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginDto dto);
    }
}
