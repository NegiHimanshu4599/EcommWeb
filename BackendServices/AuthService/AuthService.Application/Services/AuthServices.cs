using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.CommonFunctions;
using AuthService.Domain.Entities;
using AuthService.Domain.Interface;
using AuthService.Infrastructure.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthService.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly JwtOptions _jwtOptions;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IRefreshTokenRepository refreshTokenRepository,IUserProfileRepository userProfileRepository, IJwtTokenGenerator jwtTokenGenerator,IOptions<JwtOptions> jwtOptions, IMapper mapper, ILogger<AuthService> logger)
        {  
            _userManager = userManager;
            _roleManager = roleManager;
            _refreshTokenRepository = refreshTokenRepository;
            _userProfileRepository = userProfileRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtOptions = jwtOptions.Value;
            _mapper = mapper;
            _logger = logger;
        }
        #region Public Methods
        public async Task<LoginResponseDto> RegisterAsync(RegisterUserDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (dto.Password != dto.ConfirmPassword)
                throw new InvalidOperationException("Passwords do not match.");
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidOperationException("Email is already registered.");
            await using var transaction = await _refreshTokenRepository.BeginTransactionAsync();
            try
            {
                var user = _mapper.Map<ApplicationUser>(dto);
                user.UserName = dto.Email;
                user.Email = dto.Email;
                user.IsActive = true;
                var createResult = await _userManager.CreateAsync(user, dto.Password);
                EnsureIdentitySucceeded(createResult);
                string role = await DetermineUserRoleAsync();
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                EnsureIdentitySucceeded(roleResult);
                var profile = new UserProfile
                {
                    UserId = user.Id,
                    FullName = dto.UserName,
                    PrimaryPhoneNumber =string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _userProfileRepository.AddAsync(profile);
                await _userProfileRepository.SaveAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("User {Email} registered successfully.",dto.Email);
                return await BuildLoginResponseAsync(user);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex,"Registration failed for {Email}", dto.Email);
                throw;
            }
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            var user = await _userManager.Users.Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.Email == dto.Email);
            ValidateUser(user);
            bool passwordValid = await _userManager.CheckPasswordAsync(user!, dto.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Failed login attempt for {Email}. Invalid password.", dto.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            _logger.LogInformation("User {Email} logged in successfully.",dto.Email);
            return await BuildLoginResponseAsync(user!);
        }
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto); 
            var hashedToken = _jwtTokenGenerator.HashRefreshToken(dto.RefreshToken);
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(hashedToken);
            if (storedToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token.");
            if (storedToken.IsRevoked)
                throw new UnauthorizedAccessException("Refresh token has been revoked.");
            if (storedToken.ExpiryDate <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token has expired.");
            var user =await _userManager.Users.Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.Id == storedToken.UserId);
            ValidateUser(user);
            await using var transaction = await _refreshTokenRepository.BeginTransactionAsync();
            try
            {
                storedToken.IsRevoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;
                _refreshTokenRepository.Update(storedToken);
                await _refreshTokenRepository.SaveAsync();
                var response = await BuildLoginResponseAsync(user!);
                await transaction.CommitAsync();
                _logger.LogInformation("Refresh token rotated for user {UserId}.", user!.Id);
                return response;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex,"Refresh token rotation failed.");
                throw;
            }
        }
        public async Task LogoutAsync(LogoutDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            var hashedToken = _jwtTokenGenerator.HashRefreshToken(dto.RefreshToken);
            var storedToken =await _refreshTokenRepository.GetByTokenAsync(hashedToken);
            if (storedToken == null || storedToken.IsRevoked)
                return;
            await using var transaction = await _refreshTokenRepository.BeginTransactionAsync();
            try
            {
                storedToken.IsRevoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;
                _refreshTokenRepository.Update(storedToken);
                await _refreshTokenRepository.SaveAsync();
                await transaction.CommitAsync();
                _logger.LogInformation("User {UserId} logged out successfully.",storedToken.UserId);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex,"Logout failed for user {UserId}.",storedToken.UserId);
                throw;
            }
        }
        public async Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            var user = await _userManager.Users.Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null)
            {
                await using var transaction = await _refreshTokenRepository.BeginTransactionAsync();
                try
                {
                    user = new ApplicationUser
                    {
                        UserName = dto.Email,
                        Email = dto.Email,
                        EmailConfirmed = true,
                        IsActive = true
                    };
                    var createResult = await _userManager.CreateAsync(user);
                    EnsureIdentitySucceeded(createResult);
                    string role = await DetermineUserRoleAsync();
                    var roleResult = await _userManager.AddToRoleAsync(user, role);
                    EnsureIdentitySucceeded(roleResult);
                    var profile = new UserProfile
                    {
                        UserId = user.Id,
                        FullName = dto.Name,
                        PrimaryPhoneNumber = string.Empty,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _userProfileRepository.AddAsync(profile);
                    await _userProfileRepository.SaveAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation( "Google account created for {Email}.", dto.Email);
                }
                catch(Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex,"Google registration failed for {Email}.", dto.Email);
                    throw;
                }
            }
            else
            {
                ValidateUser(user);
                _logger.LogInformation("Google login successful for {Email}.", dto.Email);
            }
            return await BuildLoginResponseAsync(user);
        }
        public async Task<bool> DeactivateUserAsync(string userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            if (!user.IsActive)
                return true;
            await using var transaction = await _refreshTokenRepository.BeginTransactionAsync();
            try
            {
                user.IsActive = false;
                var result = await _userManager.UpdateAsync(user);
                EnsureIdentitySucceeded(result);
                await RevokeAllRefreshTokensAsync(user.Id);
                await transaction.CommitAsync();
                _logger.LogInformation("User {UserId} has been deactivated.",user.Id);
                return true;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex,"Failed to deactivate user {UserId}.",userId);
                throw;
            }
        }
        #endregion
        #region Private Helpers
        private static void ValidateUser(ApplicationUser? user)
        {
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials.");
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Your account has been deactivated.");
        }
        private static void EnsureIdentitySucceeded(IdentityResult result)
        {
            if (result.Succeeded)
                return;
            var errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }
        private async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
        private async Task RevokeAllRefreshTokensAsync(string userId)
        {
            await _refreshTokenRepository.RevokeAllUserTokens(userId);
        }
        private async Task<string> CreateRefreshTokenAsync(string userId)
        {
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var entity = new RefreshToken
            {
                Token = _jwtTokenGenerator.HashRefreshToken(refreshToken),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(entity);
            await _refreshTokenRepository.SaveAsync();
            return refreshToken;
        }
        private async Task<bool> IsProfileCompleteAsync(string userId)
        {
            var profile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (profile == null)
                return false;
            return
                !string.IsNullOrWhiteSpace(profile.FullName)
                &&
                !string.IsNullOrWhiteSpace(profile.PrimaryPhoneNumber);
        }
        private async Task<LoginResponseDto> BuildLoginResponseAsync(ApplicationUser user)
        {
            var roles = await GetUserRolesAsync(user);
            var accessToken =_jwtTokenGenerator.GenerateAccessToken( user,roles);
            await RevokeAllRefreshTokensAsync(user.Id);
            var refreshToken = await CreateRefreshTokenAsync(user.Id);
            var response = _mapper.Map<LoginResponseDto>(user);
            response.Role = roles.FirstOrDefault() ?? SD.Role_Individual;
            response.AccessToken = accessToken;
            response.RefreshToken = refreshToken;
            response.AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtOptions.DurationInMinutes);
            response.IsProfileComplete = await IsProfileCompleteAsync(user.Id);
            return response;
        }
        private async Task<string> DetermineUserRoleAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync(SD.Role_Admin);
            return admins.Any()? SD.Role_Individual: SD.Role_Admin;
        }
        #endregion
    }
}