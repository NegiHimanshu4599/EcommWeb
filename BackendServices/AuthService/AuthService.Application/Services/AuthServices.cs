using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Application.Security;
using AuthService.Domain.CommonFunctions;
using AuthService.Domain.Entities;
using AuthService.Domain.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace AuthService.Application.Services
{
    public class AuthServices : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<AuthServices> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly JwtTokenGenerator _jwt;
        private readonly IConfiguration _config;

        public AuthServices(IConfiguration config, IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository, JwtTokenGenerator jwt,
            ILogger<AuthServices> logger, UserManager<ApplicationUser> userManager, IMapper mapper,
            RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwt = jwt;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
        }
        public async Task<bool> DeactivateUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                throw new KeyNotFoundException("User Not Found");
            }
            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task<UserProfileDto> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                throw new KeyNotFoundException("User Not Found");
            }
            return new UserProfileDto
            {
                Email = user.Email,
                Name = user.Name,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                PhoneNumber = user.PhoneNumber
            };
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var start = DateTime.UtcNow;
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _logger.LogWarning("Email not found: {Email}", dto.Email);
                throw new UnauthorizedAccessException("Invalid Credentials");
            }
            var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isValid)
            {
                _logger.LogWarning("Invalid Password");
                throw new UnauthorizedAccessException("Invalid Credentials");
            }
            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("User is Deactivated");
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
            {
                throw new KeyNotFoundException("User has no Role Assigned");
            }
            var role = roles.FirstOrDefault() ?? "";
            _logger.LogInformation($"Login took {(DateTime.UtcNow - start).TotalMilliseconds} ms");
            return await GenerateTokensAsync(user, role);
        }
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto Dto)
        {
            var hashed = _jwt.HashToken(Dto.RefreshToken);
            var stored = await _refreshTokenRepository.FirstOrDefaultAsync(x => x.Token == hashed);
            if (stored == null || stored.IsRevoked || stored.ExpiryDate < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid refresh token attempt");
                throw new UnauthorizedAccessException("Invalid Refresh Token");
            }
            var user = await _userManager.FindByIdAsync(stored.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User Not Found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
            {
                throw new KeyNotFoundException("User has no Role Assigned");
            }
            var role = roles.FirstOrDefault() ?? "";
            var newAccess = _jwt.GenerateToken(user, role);
            var newRefresh = _jwt.GenerateRefreshToken();
            stored.IsRevoked = true;
            var newToken = new RefreshToken
            {
                Token = _jwt.HashToken(newRefresh),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(newToken);
            await _refreshTokenRepository.SaveAsync();
            return new LoginResponseDto
            {
                Email = user.Email,
                Name = user.Name,
                Role = role,
                AccessToken = newAccess,
                RefreshToken = newRefresh,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"])),
            };
        }
        public async Task<LoginResponseDto> RegisterAsync(RegisterUserDto Dto)
        {
            if (Dto.Password != Dto.ConfirmPassword)
            {
                _logger.LogWarning("Password do not match Confirm Password");
                throw new InvalidOperationException("Pasword and Confirm Password do not match");
            }
            var userExits = await _userManager.FindByEmailAsync(Dto.Email);
            if (userExits != null)
            {
                _logger.LogWarning($"{Dto.Email}is already Exists");
                throw new InvalidOperationException("User already Exists");
            }
            var user = new ApplicationUser
            {
                UserName = Dto.Email,
                Email = Dto.Email,
                Name = Dto.UserName,
            };
            var result = await _userManager.CreateAsync(user, Dto.Password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
            }
            var adminUsers = await _userManager.GetUsersInRoleAsync(SD.Role_Admin);
            string assignedRole;
            if (!adminUsers.Any())
            {
                assignedRole = SD.Role_Admin;
            }
            else
            {
                assignedRole = SD.Role_Individual;
            }
            var roleResult = await _userManager.AddToRoleAsync(user, assignedRole);
            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    string.Join(",",
                    roleResult.Errors.Select(x => x.Description)));
            }

            return await GenerateTokensAsync(user, assignedRole);
        }
        public async Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                throw new KeyNotFoundException("User Not Found");
            }
            user.Email = dto.Email;
            user.Name = dto.Name;
            user.StreetAddress = dto.StreetAddress;
            user.City = dto.City;
            user.State = dto.State;
            user.PostalCode = dto.PostalCode;
            user.PhoneNumber = dto.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Update Failed");
            }
            return new UserProfileDto
            {
                Email = user.Email,
                Name = user.Name,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                PhoneNumber = user.PhoneNumber
            };
        }
        public async Task LogoutAsync(LogoutDto dto)
        {
            var hashed = _jwt.HashToken(dto.RefreshToken);
            var stored = await _refreshTokenRepository.FirstOrDefaultAsync(x => x.Token == hashed);
            if (stored == null) return;
            var tokens = await _refreshTokenRepository.GetAllAsync(x => x.UserId == stored.UserId);
            foreach (var t in tokens)
            {
                t.IsRevoked = true;
            }
            await _refreshTokenRepository.SaveAsync();
        }
        public async Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    Name = dto.Name,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
                }
                await _userManager.AddToRoleAsync(user, SD.Role_Individual);
            }
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? SD.Role_Individual;
            return await GenerateTokensAsync(user, role);
        }
        public async Task<List<UserProfileDto>> GetAllProfileAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            if (!users.Any())
            {
                _logger.LogInformation("No users found in the system.");
                return new List<UserProfileDto>();
            }
            return users.Select(user => new UserProfileDto
            {
                Email = user.Email,
                Name = user.Name,
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                PostalCode = user.PostalCode,
                PhoneNumber = user.PhoneNumber
            }).ToList();
        }
        private async Task<LoginResponseDto> GenerateTokensAsync(ApplicationUser user, string role)
        {
            var accessToken = _jwt.GenerateToken(user, role);
            await _refreshTokenRepository.RevokeAllUserTokens(user.Id);
            var refreshToken = _jwt.GenerateRefreshToken();
            var refresh = new RefreshToken
            {
                Token = _jwt.HashToken(refreshToken),
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(refresh);
            await _refreshTokenRepository.SaveAsync();
            bool isProfileComplete =
                !string.IsNullOrWhiteSpace(user.PhoneNumber) &&
                !string.IsNullOrWhiteSpace(user.StreetAddress) &&
                !string.IsNullOrWhiteSpace(user.City) &&
                !string.IsNullOrWhiteSpace(user.State) &&
                !string.IsNullOrWhiteSpace(user.PostalCode);
            return new LoginResponseDto
            {
                Email = user.Email,
                Name = user.Name,
                Role = role,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["Jwt:DurationInMinutes"])),
                IsProfileComplete = isProfileComplete
            };
        }
    }
}
