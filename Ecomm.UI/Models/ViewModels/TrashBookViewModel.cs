using Ecomm.UI.Models.BookDtos;

namespace Ecomm.UI.Models.ViewModels
{
    public class TrashBookViewModel
    {
        public TrashDashboardDto Dashboard { get; set; }
        public PagedResult<TrashBookDto> PagedBooks { get; set; }
        public BookFilterDto Filter { get; set; }
    }
}
