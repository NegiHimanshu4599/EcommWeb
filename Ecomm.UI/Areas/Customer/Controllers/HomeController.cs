using Ecomm.UI.Common;
using Ecomm.UI.Models;
using Ecomm.UI.Models.BookDtos;
using Ecomm.UI.Models.ViewModels;
using Ecomm.UI.ServicesConnection;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ecomm.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAPIService _apiService;
        public HomeController(ILogger<HomeController> logger,IAPIService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }
        public async Task<IActionResult> Index()
        {
            var getBooks = await _apiService.GetAsync<IEnumerable<BookListDto>>(SD.BookAPIPath);
            return View(getBooks);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int Id)
        {
            var getBook = await _apiService.GetAsync<BookDetailDto>($"{ SD.BookAPIPath}/{Id}");
            return View(getBook);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
