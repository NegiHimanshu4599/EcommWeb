using Ecomm.UI.Models.BookDtos;
using Ecomm.UI.Models.CoverTypeDto;

namespace Ecomm.UI.Models.ViewModels
{
    public class CoverTypeTrashViewModel
    {
        public CoverTypeTrashDashboardDto Dashboard { get; set; }
        public PagedResult<CoverTypeTrashDto> PagedCoverTypes { get; set; }
        public BookFilterDto Filter { get; set; }
    }
}
