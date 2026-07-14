//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using NotificationService.Application.Interface.Services;
//using System.Threading.Tasks;

//namespace NotificationService.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class NotificationLogController : ControllerBase
//    {
//        private readonly INotificationLogService _notificationLogService;
//        public NotificationLogController(INotificationLogService notificationLogService)
//        {
//            _notificationLogService = notificationLogService;
//        }
//        [HttpGet]
//        public async Task<IActionResult> GetNotificationLogService(int id)
//        {
//            var notificationLogService = await _notificationLogService.GetByIdAsync(id);
//            if (notificationLogService == null)
//                return NotFound();
//            return Ok(notificationLogService);
//        }
//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetNotification(int id)
//        {
//            var Service = await _notificationLogService.GetByNotificationIdAsync(id);
//            return Ok(Service);
//        }
//    }
//}
