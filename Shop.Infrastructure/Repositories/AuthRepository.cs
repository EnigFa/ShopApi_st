using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Enums;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class AuthRepository(ShopDbContext _context) : IAuthRepository
{
    public async Task<bool> IsExistEmailAsync(string email)
    {
        var userFromDb = await _context.Users
            .FirstOrDefaultAsync(user => user.Email == email);

        if (userFromDb == null)
        {
            return false;
        }

        return true;
    }

    public async Task<User?> RegisterUserAsync(User user, string hash)
    {
        user.PasswordHash = hash;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return await _context.Users
            .FirstOrDefaultAsync(userFromDb =>
                userFromDb.Email == user.Email &&
                userFromDb.PasswordHash == user.PasswordHash);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<RefreshToken> AddRefreshTokenAsync(
        RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(
        string token)
    {
        return await _context.RefreshTokens
            .Include(refreshToken => refreshToken.User)
            .FirstOrDefaultAsync(refreshToken =>
                refreshToken.Token == token &&
                refreshToken.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<bool> HasAdminAsync()
    {
        return await _context.Users
            .AnyAsync(user => user.Role == UserRole.Admin);
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task ExpireRefreshTokensAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(token =>
                token.UserId == userId &&
                token.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.ExpiresAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task AddPasswordResetTokenAsync(PasswordResetToken token)
    {
        await _context.PasswordResetTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task InvalidatePasswordResetTokensAsync(Guid userId)
    {
        var tokens = await _context.PasswordResetTokens
            .Where(token =>
                token.UserId == userId &&
                !token.IsUsed)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsUsed = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetValidPasswordResetTokenAsync(
        string tokenHash)
    {
        return await _context.PasswordResetTokens
            .Include(token => token.User)
            .FirstOrDefaultAsync(token =>
                token.TokenHash == tokenHash &&
                !token.IsUsed &&
                token.ExpiresAt > DateTime.UtcNow);
    }
}