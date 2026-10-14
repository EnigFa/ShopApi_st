using Shop.Application.DTOs.UserDTOs;
using Shop.Application.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Interfaces.Services;

public interface IAuthService
{
    Task<(UserReadDTO? User, string? Token, string? RefreshToken)> RegisterAsync(UserCreateDTO dto);
    Task<(string? Token, string? RefreshToken)> LoginAsync(UserLoginDTO dto);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
}
