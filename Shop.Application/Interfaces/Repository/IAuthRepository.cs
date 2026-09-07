using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IAuthRepository
{
    Task<User?> RegisterUserAsync(User user, string hash);

    Task<bool> IsExistEmailAsync(string email);

    Task<User?> GetByEmailAsync(string email);

    Task<bool> HasAdminAsync();

    Task UpdateUserAsync(User user);

    Task<RefreshToken> AddRefreshTokenAsync(
        RefreshToken refreshToken);

    Task<RefreshToken?> GetValidRefreshTokenAsync(
        string token);

    Task ExpireRefreshTokensAsync(Guid userId);

    Task AddPasswordResetTokenAsync(PasswordResetToken token);

    Task InvalidatePasswordResetTokensAsync(Guid userId);

    Task<PasswordResetToken?> GetValidPasswordResetTokenAsync(
        string tokenHash);
}