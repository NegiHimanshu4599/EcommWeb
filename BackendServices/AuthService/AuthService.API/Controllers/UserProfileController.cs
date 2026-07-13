using Asp.Versioning;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound();
            var result = await _userProfileService.GetProfile(userId);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound();
            var result = await _userProfileService.UpdateProfile(userId, dto);
            return Ok(result);
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProfileAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userProfileService.GetProfile(userId);
            return Ok(result);
        }
    }
}