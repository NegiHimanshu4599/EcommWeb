namespace Ecomm.UI.Models.BookDtos
{
    public class TrashBookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedAgo => DeletedAt == null ? "No Data" : $"{(DateTime.UtcNow - DeletedAt.Value).Days} days ago";
    }
}