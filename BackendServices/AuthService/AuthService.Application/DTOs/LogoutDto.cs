using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class LogoutDto
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}