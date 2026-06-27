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
        Task<LoginResponseDto> RegisterAsync(RegisterUserDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto dto);
        Task LogoutAsync(LogoutDto dto);
        Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginDto dto);
        Task<bool> DeactivateUserAsync(string userId);
    }
}