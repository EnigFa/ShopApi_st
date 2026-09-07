using Shop.Application.DTOs.UserDTOs;
using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Services;

public interface IJWTService
{
    public string GenerateAccessToken(
        UserLoginDTO userLoginDto,
        string role);

    public RefreshToken GenerateRefreshToken(
        Guid userId);
}