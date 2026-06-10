using Ecomm.UI.Common;
using Ecomm.UI.Models.AuthDtos;
using Ecomm.UI.Models.BookDtos;
using Ecomm.UI.ServicesConnection;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly IAPIService _apiService;
        public AdminController(IAPIService apiService)
        { 
            _apiService = apiService;
        }
        public async Task<IActionResult> Index()
        {
            var getBooks = await _apiService.GetAsync<IEnumerable<BookListDto>>(SD.BookAPIPath);
            return View(getBooks);
        }
        public IActionResult Manage()
        {
            return View();
        }
        public async Task<IActionResult> GetAllUser()
        {
            var getUsers = await _apiService.GetAsync<IEnumerable<UserProfileDto>>(SD.AuthAPIPath + "/AllProfiles");
            return View(getUsers);
        }
    }
}