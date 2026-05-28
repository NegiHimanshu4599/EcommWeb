using Ecomm.UI.Common;
using Ecomm.UI.Models.BookDtos;
using Ecomm.UI.Models.CategoryDto;
using Ecomm.UI.Models.CoverTypeDto;
using Ecomm.UI.Models.ViewModels;
using Ecomm.UI.ServicesConnection;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IAPIService _apiService;
        public CoverTypeController(IAPIService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCoverType()
        {
            var covertypes = await _apiService.GetAsync<IEnumerable<CoverTypeDto>>(SD.CoverTypeAPIPath);
            return Json(new { data = covertypes });
        }
        public async Task<IActionResult> UpsertCoverType(int? id)
        {
            CoverTypeDto coverType = new CoverTypeDto();
            if (id == null) return View(coverType);
            coverType = await _apiService.GetAsync<CoverTypeDto>(SD.CoverTypeAPIPath + "/" + id);
            if (coverType == null) return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertCoverType(CoverTypeDto coverType)
        {
            if (coverType == null)
                return NotFound();
            if (!ModelState.IsValid)
                return View(coverType);
            if (coverType.Id == 0)
            {
                await _apiService.PostAsync<CoverTypeDto>(SD.CoverTypeAPIPath, coverType);
            }
            else
            {
                await _apiService.PutAsync<CoverTypeDto>(SD.CoverTypeAPIPath, coverType);
            }
            return RedirectToAction("Manage","Admin");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteCoverType(int id)
        {
            try
            {
                await _apiService.DeleteAsync(SD.CoverTypeAPIPath + "/" + id);
                return Json(new { success = true, message = "CoverType Deleted Successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Something went Wrong data !!",ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> PermanentDelete(string? search, int pageNumber = 1)
        {
            var filter = new BookFilterDto
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = 5,
                IsDeletedPage = true
            };
            var pagedCoverTypes = await _apiService.PostAsync<PagedResult<CoverTypeTrashDto>>($"{SD.CoverTypeAPIPath}/trash", filter);
            var dashboard = await _apiService.GetAsync<CoverTypeTrashDashboardDto>($"{SD.CoverTypeAPIPath}/trash-dashboard");
            var vm = new CoverTypeTrashViewModel
            {
                Dashboard = dashboard,
                PagedCoverTypes = pagedCoverTypes,
                Filter = filter
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var success = await _apiService.deleteAsync($"{SD.CoverTypeAPIPath}/{id}/permanent");
            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to delete covertype";
            }
            else
            {
                TempData["SuccessMessage"] = "Covertype permanently deleted successfully";
            }
            return RedirectToAction(nameof(PermanentDelete));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            await _apiService.PatchAsync($"{SD.CoverTypeAPIPath}/{id}/restore");
            TempData["SuccessMessage"] = "Covertype restored successfully";
            return RedirectToAction(nameof(PermanentDelete));
        }
    }
}