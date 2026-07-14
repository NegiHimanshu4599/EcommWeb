using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class LogoutDto
    {
        public string RefreshToken { get; set; } = null!;
    }
}