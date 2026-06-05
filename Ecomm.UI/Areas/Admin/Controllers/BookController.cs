using Ecomm.UI.Common;
using Ecomm.UI.Models.BookDtos;
using Ecomm.UI.Models.CategoryDto;
using Ecomm.UI.Models.CoverTypeDto;
using Ecomm.UI.Models.ViewModels;
using Ecomm.UI.ServicesConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecomm.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly IAPIService _apiService;
        public BookController(IAPIService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> PermanentDelete(string? search,int pageNumber = 1,string? sortBy = null, bool isDescending = false)
        {
            var filter = new BookFilterDto
            {
                Search = search,
                PageNumber = pageNumber,
                PageSize = 5,
                SortBy = sortBy,
                IsDescending = isDescending,
                IsDeletedPage = true
            };
            var pagedBooks =await _apiService.PostAsync<PagedResult<TrashBookDto>>($"{SD.BookAPIPath}/trash",filter);
            var dashboard = await _apiService.GetAsync<TrashDashboardDto>($"{SD.BookAPIPath}/trash-dashboard");
            var vm = new TrashBookViewModel
            {
                Dashboard = dashboard,
                PagedBooks = pagedBooks,
                Filter = filter
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDelete(int id)
        {
            var success = await _apiService.deleteAsync($"{SD.BookAPIPath}/{id}/permanent");
            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to delete data";
            }
            else
            {
                TempData["SuccessMessage"] = "Book deleted successfully";
            }
            return RedirectToAction(nameof(PermanentDelete));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            await _apiService.PatchAsync($"{SD.BookAPIPath}/{id}/restore");
            TempData["SuccessMessage"] ="Book restored successfully";
            return RedirectToAction(nameof(PermanentDelete));
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var getBooks = await _apiService.GetAsync<IEnumerable<BookListDto>>(SD.BookAPIPath);
            return Json(new { data = getBooks });
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpDelete]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _apiService.DeleteAsync(SD.BookAPIPath + "/" + id);
                return Json(new{success = true,message = "Book Deleted Successfully"});
            }
            catch (Exception ex)
            {
                return Json(new {success = false,message = "Something went Wrong data !!",ex.Message });
            }
        }
        public async Task<IActionResult> UpsertBook(int? id)
        {
            var vm = new BookViewModel();
            await LoadDropdowns(vm);
            vm.Books = new UpsertBook();
            if (id == null || id == 0)
                return View(vm);
            var book = await _apiService.GetAsync<BookDetailDto>(
                SD.BookAPIPath + "/" + id);
            vm.Books = new UpsertBook
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Price = book.Price,
                CategoryId = book.CategoryId,
                CoverTypeId = book.CoverTypeId,
                StockQuantity = book.StockQuantity,
                ImageUrl = book.ImageUrl
            };
            return View(vm);
        }
        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertBook(BookViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(vm);
                return View(vm);
            }
            if (vm.Books.Id == 0)
            {
                var createDto = new CreateBookDto
                {
                    Title = vm.Books.Title,
                    Author = vm.Books.Author,
                    ISBN = vm.Books.ISBN,
                    Description = vm.Books.Description,
                    Price = vm.Books.Price,
                    CategoryId = vm.Books.CategoryId,
                    CoverTypeId = vm.Books.CoverTypeId,
                    StockQuantity = vm.Books.StockQuantity,
                    ImageFile = vm.Books.ImageFile
                };
                await _apiService.PostMultipartAsync<CreateBookDto>(SD.BookAPIPath, createDto);
            }
            else
            {
                var updatedto = new UpdateBookDto
                {
                    Id = vm.Books.Id,
                    Title = vm.Books.Title,
                    Author = vm.Books.Author,
                    ISBN = vm.Books.ISBN,
                    Description = vm.Books.Description,
                    Price = vm.Books.Price,
                    CategoryId = vm.Books.CategoryId,
                    CoverTypeId = vm.Books.CoverTypeId,
                    StockQuantity = vm.Books.StockQuantity,
                    ImageFile = vm.Books.ImageFile,
                    ImageUrl = vm.Books.ImageUrl
                };
                await _apiService.PutMultipartAsync(SD.BookAPIPath,updatedto);
            }
            return RedirectToAction("Manage", "Admin");
        }
        private async Task LoadDropdowns(BookViewModel vm)
        {
            var categories = await _apiService.GetAsync<IEnumerable<CategoryDto>>(SD.CategoryAPIPath);
            var covers = await _apiService.GetAsync<IEnumerable<CoverTypeDto>>(SD.CoverTypeAPIPath);
            vm.CategoryList = categories.Where(x => x.ParentCategoryId != null).Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString()
                });
            vm.CoverTypeList = covers.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });
        }
    }
}