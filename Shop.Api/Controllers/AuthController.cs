using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Exceptions.HandlerExceptions;
using Shop.Application.DTOs.AuthDTOs;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]

public class AuthController(IAuthService _authService, IWebHostEnvironment _environment):ControllerBase
{
    // Вхід через Google
    [HttpGet("login-google")]
    public IActionResult LoginGoogle()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(ExternalResponse))
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    // Зворотний виклик після успішної авторизації
    [HttpGet("external-response")]
    public async Task<IActionResult> ExternalResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return BadRequest("Помилка зовнішньої аутентифікації.");

        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var providerId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        // Тут зазвичай виконується:
        // 1. Пошук користувача в БД за email/providerId.
        // 2. Реєстрація нового користувача, якщо його немає.
        // 3. Генерація власного JWT  (refresh and access)

        return Ok(new { Name = name, Email = email, ProviderId = providerId });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok("Вихід успішний.");
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserCreateDTO dto)
    {
        var user = await _authService.RegisterAsync(dto);
        if (user.User == null || user.Token == null || user.RefreshToken == null)
            throw new BadRequestException("Користувач за таким email вже існує");


        Response.Cookies.Append(
            "refreshToken",
            user.RefreshToken,
            CreateRefreshCookieOptions());
        return Ok(new { user = user.User, token = user.Token, refreshToken = user.RefreshToken });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] UserLoginDTO dto)
    {
        var user = await _authService.LoginAsync(dto);
        if (user.Token == null || user.RefreshToken == null)
            throw new UnauthorizedAppException("Невірний email або пароль");


        Response.Cookies.Append(
            "refreshToken",
            user.RefreshToken,
            CreateRefreshCookieOptions());
        return Ok(new { token = user.Token, refreshToken = user.RefreshToken });
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest? dto)
    {
        var refreshToken = dto?.RefreshToken;

        if (string.IsNullOrEmpty(refreshToken))
            Request.Cookies.TryGetValue("refreshToken", out refreshToken);

        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedAppException("Refresh token not found.");

        var result = await _authService.RefreshTokenAsync(refreshToken);
        if(result==null)
            throw new UnauthorizedAppException("Refresh token not created.");
        Response.Cookies.Append(
           "refreshToken",
           result.RefreshToken,
           CreateRefreshCookieOptions());
        return Ok(new { token = result.AccessToken, refreshToken = result.RefreshToken });
    }

    private CookieOptions CreateRefreshCookieOptions()
    {
        var isDevelopment = _environment.IsDevelopment();

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment,
            SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.Strict,
            Path = "/"
        };
    }

    [Authorize]
    [HttpGet]
    public IActionResult Profile()
    {
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Email = email,
            Role = role
        });
    }

}
