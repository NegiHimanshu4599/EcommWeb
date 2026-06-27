using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class UpdateUserProfileDto
    {
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = null!;
        [Required]
        [Phone]
        [MaxLength(20)]
        public string PrimaryPhoneNumber { get; set; } = null!;
    }
}