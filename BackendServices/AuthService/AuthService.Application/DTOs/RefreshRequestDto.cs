using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = null!;
    }
}