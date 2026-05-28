namespace BookService.Domain.Entities.Category
{
    public class RestoredCategoryHistory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime RestoredAt { get; set; }
    }
}