using Shop.Application.DTOs.UserDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IAuthService
{
    Task<(
        UserReadDTO? User,
        string? Token,
        string? RefreshToken)> RegisterAsync(
            UserCreateDTO dto);

    Task<(
        UserReadDTO? User,
        string? Token,
        string? RefreshToken)> LoginAsync(
            UserLoginDTO dto);

    Task<string?> RefreshAccessTokenAsync(
        string refreshToken);

    Task EnsureFirstAdminAsync(string email, string password);

    Task<UserReadDTO?> CreateStaffAsync(StaffCreateDTO dto);

    Task RequestPasswordResetAsync(string email);

    Task<bool> ResetPasswordAsync(ResetPasswordDTO dto);
}