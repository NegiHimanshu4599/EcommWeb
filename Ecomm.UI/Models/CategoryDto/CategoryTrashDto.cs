namespace Ecomm.UI.Models.CategoryDto
{
    public class CategoryTrashDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}