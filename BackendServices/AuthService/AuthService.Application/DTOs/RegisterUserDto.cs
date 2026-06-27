using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [MaxLength(150)]
        public string UserName { get; set; } = null!;
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = null!;
        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        public string Password { get; set; } = null!;
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
    }
}