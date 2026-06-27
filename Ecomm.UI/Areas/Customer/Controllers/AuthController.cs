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
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var result = await _apiService.PostAsync<LoginResponseDto>(SD.AuthAPIPath + "/login",model);
                if (result == null || string.IsNullOrEmpty(result.AccessToken))
                {
                    ModelState.AddModelError("", "Invalid email or password");
                    return View(model);
                }
                SetAuthCookies(result);
                await SignInUser(result.AccessToken);
                if (!result.IsProfileComplete)
                {
                    return RedirectToAction(nameof(CompleteProfile));
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                var result = await _apiService.PostAsync<LoginResponseDto>(SD.AuthAPIPath + "/Register",model);
                if (result == null)
                {
                    ModelState.AddModelError("", "Registration Failed");
                    return View(model);
                }
                SetAuthCookies(result);
                await SignInUser(result.AccessToken);
                if (!result.IsProfileComplete)
                {
                    return RedirectToAction(nameof(CompleteProfile));
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["RefreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await _apiService.PostAsync<object>( SD.AuthAPIPath + "/logout",new
                    {
                        RefreshToken = refreshToken
                    });
                }
            }
            catch
            {}
            Response.Cookies.Delete("JWT");
            Response.Cookies.Delete("RefreshToken");
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
            if (name == null)
                return RedirectToAction("Login");
            var googleDto = new GoogleLoginDto
            {
                Email = email,
                Name = name
            };
            var response = await _apiService.PostAsync<LoginResponseDto>(SD.AuthAPIPath + "/google-login", googleDto);
            if (response != null)
            {
                SetAuthCookies(response);
                await SignInUser(response.AccessToken);
            }
            else
            {
                ModelState.AddModelError("", "Google login failed");
                return RedirectToAction(nameof(Login));
            }
            if (!response.IsProfileComplete)
            {
                return RedirectToAction("CompleteProfile");
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        private void SetAuthCookies( LoginResponseDto result)
        {
            Response.Cookies.Append("JWT",result.AccessToken,new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
                });
            Response.Cookies.Append("RefreshToken",result.RefreshToken,new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
        private async Task SignInUser(string accessToken)
        {
            var handler =new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            var claims =jwt.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
            var identity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme,principal);
        }
    }
}