namespace Ecomm.UI.Models.CoverTypeDto
{
    public class CoverTypeTrashDashboardDto
    {
        public int TotalDeletedCoverTypes { get; set; }
        public int RestoredCoverTypes { get; set; }
        public int PermanentlyDeletedCoverTypes { get; set; }
        public DateTime? LastDeletedAt { get; set; }
    }
}
