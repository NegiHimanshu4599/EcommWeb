  using Ecomm.UI.Common;
using Ecomm.UI.Models.AuthDtos;
using Ecomm.UI.ServicesConnection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecomm.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthController : Controller
    {
        private readonly IAPIService _apiService;
        public AuthController(IAPIService apiService)
        {
            _apiService = apiService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var result = await _apiService.PostAsync<LoginResponseDto>(SD.AuthAPIPath + "/login", model);
            if (result != null && !string.IsNullOrEmpty(result.AccessToken))
            {
                HttpContext.Session.SetString("JWT", result.AccessToken);
                HttpContext.Session.SetString("RefreshToken", result.RefreshToken);
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(result.AccessToken);
                var claims = jwt.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid login");
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto model)
        {
            var result = await _apiService.PostAsync<RegisterResponseDto>(SD.AuthAPIPath + "/Register", model);
            if(result != null)
            {
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "Registration Failed");
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _apiService.PostAsync<object>(SD.AuthAPIPath + "/logout", new { RefreshToken = refreshToken });
            }
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> CompleteProfile()
        {
            var getUserData = await _apiService.GetAsync<UserProfileDto>(SD.AuthAPIPath + "/Profile");
            if (getUserData == null)
                return NotFound();           
            return View(getUserData);
        }
        [HttpPost]
        public async Task<IActionResult> CompleteProfile(UpdateUserDto model)
        {
            var result = await _apiService.PutAsync<UpdateUserDto>(SD.AuthAPIPath + "/updateProfile", model);
            if (result != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Registration Failed");
            return View(model);
        }
        [HttpPost]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { area = "Customer" }, Request.Scheme);
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
            if (email == null)
                return RedirectToAction("Login");
            var googleDto = new GoogleLoginDto
            {
                Email = email,
                Name = name
            };
            var response = await _apiService.PostAsync<LoginResponseDto>(
                SD.AuthAPIPath + "/google-login", googleDto);
            if (response != null)
            {
                HttpContext.Session.SetString("JWT", response.AccessToken);
                HttpContext.Session.SetString("RefreshToken", response.RefreshToken);
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(response.AccessToken);
                var claims = jwt.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();

                var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
            }
            if (!response.IsProfileComplete)
            {
                return RedirectToAction("CompleteProfile");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}