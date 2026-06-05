using System.ComponentModel.DataAnnotations;

namespace Ecomm.UI.Models.CartDto
{
    public class UpdateCartDto
    {
        [Required]
        public int BookId { get; set; }
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
