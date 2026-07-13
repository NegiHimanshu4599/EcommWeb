using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTOs
{
    public class UpdateUserProfileDto
    {
        public string FullName { get; set; } = null!;
        public string PrimaryPhoneNumber { get; set; } = null!;
    }
}