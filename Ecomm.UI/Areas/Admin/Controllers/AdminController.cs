using Ecomm.UI.Common;
using Ecomm.UI.Models.BookDtos;
using Ecomm.UI.Models.CategoryDto;
using Ecomm.UI.Models.CoverTypeDto;
using Ecomm.UI.Models.ViewModels;
using Ecomm.UI.ServicesConnection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
    }
}