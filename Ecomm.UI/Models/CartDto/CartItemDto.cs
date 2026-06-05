namespace Ecomm.UI.Models.CartDto
{
    public class CartItemDto
    {
        public int BookId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}
