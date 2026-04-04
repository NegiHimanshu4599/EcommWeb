using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using AuthService.Application.Security;
using AuthService.Domain.CommonFunctions;
using AuthService.Domain.Entities;
using AuthService.Domain.Interface;

namespace AuthService.Application.Services
{
    public class AuthServices : IAuthService
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly ILogger<AuthServices> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly JwtTokenGenerator _jwt;
        private readonly IConfiguration _config;

        public AuthServices(IConfiguration config, IUnitOfWork unitofwork,JwtTokenGenerator jwt, 
            ILogger<AuthServices> logger,UserManager<ApplicationUser> userManager, IMapper mapper, 
            RoleManager<IdentityRole> roleManager)
        {
            _unitofwork = unitofwork;
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
            if(user == null)
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
            using var transaction = await _unitofwork.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Email not found: {Email}", dto.Email);
                        throw new UnauthorizedAccessException("Invalid Credentials");
                }
                //if (!user.EmailConfirmed)
                //{
                //    _logger.LogWarning($"Please Confirm Your Email{dto.Email}");
                //    throw new UnauthorizedAccessException("Please Confirm Your Email");
                //}
                var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
                if (!isValid)
                {
                    _logger.LogWarning("Invalid Password");
                    throw new UnauthorizedAccessException("Invalid Credentials");
                }
                if (!user.IsActive)
                {
                    _logger.LogWarning($"User {dto.Email} is Deactivated");
                    throw new UnauthorizedAccessException("User is Deactivated");
                }
                var roles = await _userManager.GetRolesAsync(user);
                if(!roles.Any())
                {
                    _logger.LogWarning("User has no Role Assigned: {UserId}", user.Id);
                    throw new KeyNotFoundException("User has no Role Assigned");
                }
                var role = roles.FirstOrDefault() ?? "" ;
                var accessToken = _jwt.GenerateToken(user, role);
                var existingRefreshTokens = await _unitofwork.RefreshToken
                    .GetActiveTokensByUserId(user.Id);
                foreach (var token in existingRefreshTokens)
                {
                    token.IsRevoked = true;
                }
                var refreshToken = _jwt.GenerateRefreshToken();
                var refresh = new RefreshToken
                {
                    Token = _jwt.HashToken(refreshToken),
                    UserId = user.Id,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false

                };
                await _unitofwork.RefreshToken.AddAsync(refresh);
                await _unitofwork.SaveAsync();
                await transaction.CommitAsync();
                return new LoginResponseDto
                {
                    Email = user.Email,
                    Name = user.Name,
                    StreetAddress = user.StreetAddress,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    State = user.State,
                    PhoneNumber = user.PhoneNumber,
                    Role = role,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                        double.Parse(_config["Jwt:DurationInMinutes"])),
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshRequestDto Dto)
        {
            using var transaction = await _unitofwork.BeginTransactionAsync();
            try
            {
                var hashed = _jwt.HashToken(Dto.RefreshToken);
                var stored = await _unitofwork.RefreshToken.FirstOrDefaultAsync(x => x.Token == hashed);
                if (stored == null || stored.IsRevoked || stored.ExpiryDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("Invalid refresh token attempt");
                    throw new UnauthorizedAccessException("Invalid Refresh Token");
                }
                var user = await _userManager.FindByIdAsync(stored.UserId);
                if (user == null)
                {
                    _logger.LogWarning("User Not Found: {UserId}", stored.UserId);
                    throw new UnauthorizedAccessException("User Not Found");
                }
                var roles = await _userManager.GetRolesAsync(user);
                if(!roles.Any())
                {
                    _logger.LogWarning("User has no Role Assigned: {UserId}", user.Id);
                    throw new KeyNotFoundException("User has no Role Assigned");
                }
                var role = roles.FirstOrDefault() ?? "" ;
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
                await _unitofwork.RefreshToken.AddAsync(newToken);
                await _unitofwork.SaveAsync();
                await transaction.CommitAsync();
                return new LoginResponseDto
                {
                    Email = user.Email,
                    Name = user.Name,
                    StreetAddress = user.StreetAddress,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    State = user.State,
                    PhoneNumber = user.PhoneNumber,
                    Role = role,
                    AccessToken = newAccess,
                    RefreshToken = newRefresh,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                        double.Parse(_config["Jwt:DurationInMinutes"]))
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<LoginResponseDto> RegisterAsync(RegisterUserDto Dto)
        {
            using var transaction = await _unitofwork.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Checking if Password Match ConfirmPassword");
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
                    StreetAddress = Dto.StreetAddress,
                    City = Dto.City,
                    PostalCode = Dto.PostalCode,
                    State = Dto.State,
                    PhoneNumber = Dto.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, Dto.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("New User is Not Created");
                    throw new Exception(string.Join(",", result.Errors.Select(x => x.Description)));
                }
                _logger.LogInformation("Checking if Admin user Exists");
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
                await _userManager.AddToRoleAsync(user, assignedRole);
                await transaction.CommitAsync();
                //return _mapper.Map<LoginResponseDto>(user);
                return new LoginResponseDto
                {
                    Email = user.Email,
                    Name = user.Name,
                    StreetAddress = user.StreetAddress,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    State = user.State,
                    PhoneNumber = user.PhoneNumber,
                    Role = assignedRole
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
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
                _logger.LogWarning("Update Failed");
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
    }
}
