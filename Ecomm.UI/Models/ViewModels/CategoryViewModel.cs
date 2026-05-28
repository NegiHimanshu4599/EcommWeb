using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecomm.UI.Models.ViewModels
{
    public class CategoryViewModel
    {
        public CategoryDto.CategoryDto Category { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ParentCategories { get; set; }
    }
}
