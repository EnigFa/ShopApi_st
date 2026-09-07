using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService _authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] UserCreateDTO dto)
    {
        var user = await _authService.RegisterAsync(dto);

        if (user.User == null ||
            user.Token == null ||
            user.RefreshToken == null)
        {
            return BadRequest("Користувач за таким email вже існує");
        }

        Response.Cookies.Append(
            "refreshToken",
            user.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

        return Ok(new
        {
            user = user.User,
            token = user.Token,
            refresh_token = user.RefreshToken
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(
        [FromBody] UserLoginDTO dto)
    {
        var user = await _authService.LoginAsync(dto);

        if (user.User == null ||
            user.Token == null ||
            user.RefreshToken == null)
        {
            return Unauthorized("Невірний email або пароль");
        }

        Response.Cookies.Append(
            "refreshToken",
            user.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

        return Ok(new
        {
            user = user.User,
            token = user.Token,
            refresh_token = user.RefreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshTokenExists = Request.Cookies.TryGetValue(
            "refreshToken",
            out var refreshToken);

        if (!refreshTokenExists ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized("Refresh token відсутній");
        }

        var newAccessToken =
            await _authService.RefreshAccessTokenAsync(refreshToken);

        if (newAccessToken == null)
        {
            return Unauthorized("Refresh token недійсний або прострочений");
        }

        return Ok(new
        {
            token = newAccessToken
        });
    }

    [Authorize]
    [HttpGet]
    public IActionResult Profile()
    {
        var email = User.FindFirst(
            System.Security.Claims.ClaimTypes.Email)?.Value;

        var role = User.FindFirst(
            System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Email = email,
            Role = role
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("staff")]
    public async Task<IActionResult> CreateStaff(
        [FromBody] StaffCreateDTO dto)
    {
        var user = await _authService.CreateStaffAsync(dto);

        if (user == null)
        {
            return BadRequest(
                "Не вдалося створити користувача. Email уже зайнятий або роль має бути Admin чи Moderator.");
        }

        return Ok(user);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDTO dto)
    {
        await _authService.RequestPasswordResetAsync(dto.Email);

        return Ok(new
        {
            message = "Якщо обліковий запис існує, лист для відновлення пароля надіслано."
        });
    }

    [Authorize(Roles = "Admin,Moderator")]
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset(
        [FromBody] ForgotPasswordDTO dto)
    {
        await _authService.RequestPasswordResetAsync(dto.Email);

        return Ok(new
        {
            message = "Якщо обліковий запис існує, лист для відновлення пароля надіслано."
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDTO dto)
    {
        var isReset = await _authService.ResetPasswordAsync(dto);

        if (!isReset)
        {
            return BadRequest("Токен недійсний або прострочений");
        }

        return Ok(new
        {
            message = "Пароль успішно змінено"
        });
    }
}