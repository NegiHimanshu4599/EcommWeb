namespace Ecomm.UI.Models.WishlistDto
{
    public class WishlistDto
    {
        public int WishlistId { get; set; }
        public string UserId { get; set; }
        public List<WishlistItemDto> Items { get; set; } = new();
    }
}
