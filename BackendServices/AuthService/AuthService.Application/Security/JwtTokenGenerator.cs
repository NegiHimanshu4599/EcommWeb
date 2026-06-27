using AuthService.Application.Interfaces;
using AuthService.Application.Options;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Application.Security
{
    public sealed class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;
        private static readonly JwtSecurityTokenHandler TokenHandler = new();

        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ValidateOptions();
        }
        public string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(roles);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty)
        };
            foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct())
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.DurationInMinutes),
                signingCredentials: credentials);
            return TokenHandler.WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
        public string HashRefreshToken(string refreshToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return Convert.ToBase64String(hash);
        }
        private void ValidateOptions()
        {
            if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
                throw new InvalidOperationException("JWT Key is missing.");
            if (Encoding.UTF8.GetByteCount(_jwtOptions.Key) < 32)
                throw new InvalidOperationException("JWT Key must be at least 32 bytes.");
            if (string.IsNullOrWhiteSpace(_jwtOptions.Issuer))
                throw new InvalidOperationException("JWT Issuer is missing.");
            if (string.IsNullOrWhiteSpace(_jwtOptions.Audience))
                throw new InvalidOperationException("JWT Audience is missing.");
            if (_jwtOptions.DurationInMinutes <= 0)
                throw new InvalidOperationException("JWT DurationInMinutes must be greater than zero.");
        }
    }
}