using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}