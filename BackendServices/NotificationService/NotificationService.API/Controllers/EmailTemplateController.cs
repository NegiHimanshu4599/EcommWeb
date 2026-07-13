//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using NotificationService.Application.Interface.Services;
//using System.Threading.Tasks;

//namespace NotificationService.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EmailTemplateController : ControllerBase
//    {
//        private readonly IEmailTemplateService _emailTemplateService;
//        public EmailTemplateController(IEmailTemplateService emailTemplateService)
//        {
//            _emailTemplateService = emailTemplateService;
//        }
//        [HttpGet]
//        public async Task<IActionResult> GetEmailTemplates()
//        {
//            var templates = await _emailTemplateService.GetAllAsync();
//            return Ok(templates);
//        }
//        [HttpGet("Active")]
//        public async Task<IActionResult> GetActiveEmailTemplate()
//        {
//            var activeTemplate = await _emailTemplateService.GetActiveAsync();
//            return Ok(activeTemplate);
//        }
//        [HttpGet("deactiveEmailTemplates")]
//        public async Task<IActionResult> DeactiveTemplates(int id)
//        {
//            return Ok(  _emailTemplateService.DeactivateAsync(id));
//        }
//    }
//}
