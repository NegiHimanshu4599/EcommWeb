namespace AuthService.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime AccessTokenExpiry { get; set; }
        public bool IsProfileComplete { get; set; }
    }
}