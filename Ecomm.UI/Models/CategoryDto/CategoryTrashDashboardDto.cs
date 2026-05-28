namespace Ecomm.UI.Models.CategoryDto
{
    public class CategoryTrashDashboardDto
    {
        public int TotalDeletedCategories { get; set; }
        public int RestoredCategories { get; set; }
        public int PermanentlyDeletedCategories { get; set; }
        public DateTime? LastDeletedAt { get; set; }
    }
}