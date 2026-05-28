using Ecomm.UI.Common;
using Ecomm.UI.Models.BookDtos;
using Ecomm.UI.Models.CategoryDto;
using Ecomm.UI.Models.ViewModels;
using Ecomm.UI.ServicesConnection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecomm.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IAPIService _apiService;
        public CategoryController(IAPIService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var categories = await _apiService.GetAsync<IEnumerable<CategoryDto>> (SD.CategoryAPIPath);
            return Json(new { data = categories });
        }
        public async Task<IActionResult> UpsertCategory(int? id)
        {
            var vm = new CategoryViewModel
            {
                Category = new CategoryDto()
            };
            await LoadParentCategories(vm);
            if (id != null && id != 0)
            {
                vm.Category = await _apiService.GetAsync<CategoryDto>(SD.CategoryAPIPath + "/" + id);
                await LoadParentCategories(vm);
            }
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertCategory(CategoryViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadParentCategories(vm);
                return View(vm);
            }
            if (vm.Category.Id == 0)
            {
                await _apiService.PostAsync<CategoryDto>(SD.CategoryAPIPath,vm.Category);
            }
            else
            {
                await _apiService.PutAsync<CategoryDto>(SD.CategoryAPIPath,vm.Category);
            }
            return RedirectToAction("Manage", "Admin");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _apiService.DeleteAsync(SD.CategoryAPIPath + "/" + id);
                return Json(new { success = true, message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message.Contains("subcategories")? "Cannot delete category with subcategories": ex.Message});
            }
        }
        private async Task LoadParentCategories(CategoryViewModel vm)
        {
            var categories =await _apiService.GetAsync<IEnumerable<CategoryDto>>(SD.CategoryAPIPath);

            vm.ParentCategories = categories
                .Where(x =>x.ParentCategoryId == null&& x.Id != vm.Category.Id).Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString()
                });
        }
        [HttpGet]
        public async Task<IActionResult>PermanentDelete(string? search,int pageNumber = 1)
        {
            var filter = new BookFilterDto
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = 5,
                IsDeletedPage = true
            };
            var pagedCategories =await _apiService.PostAsync<PagedResult<CategoryTrashDto>>($"{SD.CategoryAPIPath}/trash", filter);
            var dashboard = await _apiService.GetAsync<CategoryTrashDashboardDto>($"{SD.CategoryAPIPath}/trash-dashboard");
            var vm = new CategoryTrashViewModel
            {
                Dashboard = dashboard,
                PagedCategories = pagedCategories,
                Filter = filter
            };            
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var success = await _apiService.deleteAsync($"{SD.CategoryAPIPath}/{id}/permanent");
            if (!success)
            {
                TempData["ErrorMessage"] ="Unable to delete category";
            }
            else
            {
                TempData["SuccessMessage"] ="Category permanently deleted successfully";
            }
            return RedirectToAction(nameof(PermanentDelete));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            await _apiService.PatchAsync($"{SD.CategoryAPIPath}/{id}/restore");
            TempData["SuccessMessage"] ="Category restored successfully";
            return RedirectToAction(nameof(PermanentDelete));
        }
    }
}