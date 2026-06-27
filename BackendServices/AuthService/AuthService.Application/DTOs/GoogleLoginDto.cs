using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class GoogleLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;
    }
}