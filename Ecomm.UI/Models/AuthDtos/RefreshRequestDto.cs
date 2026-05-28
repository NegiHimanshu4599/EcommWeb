using System.ComponentModel.DataAnnotations;

namespace Ecomm.UI.Models.AuthDtos
{
    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}