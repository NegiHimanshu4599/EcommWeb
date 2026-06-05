using System.ComponentModel.DataAnnotations;

namespace Ecomm.UI.Models.CartDto
{
    public class AddToCartDto
    {
        public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
