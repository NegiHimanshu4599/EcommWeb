namespace Ecomm.UI.Models.BookDtos
{
    public class TrashDashboardDto
    {
        public int TotalDeletedBooks { get; set; }
        public int RestoredBooks { get; set; }
        public int PermanentlyDeletedBooks { get; set; }
        public DateTime? LastDeletedAt { get; set; }
    }
}
