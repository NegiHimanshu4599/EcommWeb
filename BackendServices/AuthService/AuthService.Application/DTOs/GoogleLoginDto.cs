using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class GoogleLoginDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}