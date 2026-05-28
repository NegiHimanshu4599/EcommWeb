namespace Ecomm.UI.Models.CategoryDto
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public bool IsParentCategory { get; set; }
        public string? DisplayName { get; set; }
    }
}