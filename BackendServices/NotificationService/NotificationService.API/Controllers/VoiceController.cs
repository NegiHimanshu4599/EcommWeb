using Microsoft.AspNetCore.Mvc;
using System.Security;
using System.Text;

namespace NotificationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class VoiceController : ControllerBase
    {
        [HttpGet("otp")]
        [Produces("application/xml")]
        public IActionResult Otp([FromQuery] string otp)
        {
            if (string.IsNullOrWhiteSpace(otp))
                return BadRequest();
            var escapedOtp = SecurityElement.Escape(otp);
            var xml = $"""
                <Response>      
                <Say>
                Your verification code is {escapedOtp}.
                I repeat,
                your verification code is {escapedOtp}.
                This code expires in ten minutes.
                </Say>
                </Response>
                """;
            return Content(xml, "application/xml", Encoding.UTF8);
        }
        [HttpGet("message")]
        [Produces("application/xml")]
        public IActionResult Message([FromQuery] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest();
            var escaped = SecurityElement.Escape(text);
            var xml = $"""
                <Response>               
                <Say>{escaped}</Say>
                </Response>
                """;            
            return Content(xml, "application/xml", Encoding.UTF8);
        }
    }
}