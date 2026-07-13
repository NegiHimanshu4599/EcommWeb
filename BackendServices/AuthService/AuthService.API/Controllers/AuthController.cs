using Asp.Versioning;
using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshRequestDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            var user = await _authService.RegisterAsync(dto);
            return Ok(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("deactivate/{userId}")]
        public async Task<IActionResult> Deactivate(string userId)
        {
            var result = await _authService.DeactivateUserAsync(userId);
            return Ok(result);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutDto dto)
        {
            await _authService.LogoutAsync(dto);
            return Ok(new { message = "Logged out successfully" });
        }
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto model)
        {
            var result = await _authService.GoogleLoginAsync(model);
            return Ok(result);
        }
    }
}