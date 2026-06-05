using Ecomm.UI.Models.CartDto;

namespace Ecomm.UI.Models.ViewModels
{
    public class CartVM
    {
        public AddToCartDto addCart { get; set; }
        public UpdateCartDto updateCart { get; set; }
        public CartDto.CartDto cart { get; set; }
        public CartItemDto cartItem { get; set; }
    }
}
