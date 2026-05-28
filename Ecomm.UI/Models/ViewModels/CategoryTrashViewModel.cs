using Ecomm.UI.Models.BookDtos;
using Ecomm.UI.Models.CategoryDto;

namespace Ecomm.UI.Models.ViewModels
{
    public class CategoryTrashViewModel
    {
        public CategoryTrashDashboardDto Dashboard { get; set; }

        public PagedResult<CategoryTrashDto> PagedCategories { get; set; }

        public BookFilterDto Filter { get; set; }
    }
}