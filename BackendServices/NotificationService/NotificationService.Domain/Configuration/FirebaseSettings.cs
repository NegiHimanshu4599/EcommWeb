using System.ComponentModel.DataAnnotations;

namespace NotificationService.Domain.Configuration
{
    public class FirebaseSettings
    {
        public const string SectionName = "FirebaseSettings";
        [Required]
        public string ServiceAccountPath { get; set; } = null!;
        [Required]
        public string ProjectId { get; set; } = null!;
    }
}
