namespace Ecomm.UI.Models.BookDtos
{
    public class BookDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public string CoverTypeName { get; set; }
        public int CategoryId { get; set; }
        public int CoverTypeId { get; set; }
    }
}