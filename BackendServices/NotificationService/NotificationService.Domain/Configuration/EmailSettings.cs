using System.ComponentModel.DataAnnotations;

namespace NotificationService.Domain.Configuration
{
    public class EmailSettings
    {
        [Required]
        public string Host { get; set; } = null!;
        [Range(1, 65535)]
        public int Port { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]  
        public string From { get; set; } = null!;
    }
}