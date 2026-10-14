using Microsoft.Identity.Client;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Infrastructure.Repositories;

public class RefreshTokenRepository(ShopDbContext _context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken)
    {
        var now = DateTime.UtcNow;
        if (refreshToken.Id == Guid.Empty)
        {
            refreshToken.Id = Guid.NewGuid();
        }

        if (refreshToken.CreatedAt == default)
        {
            refreshToken.CreatedAt = now;
        }

        refreshToken.UpdatedAt = now;

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(RefreshToken refreshToken)
    {
        throw new NotImplementedException();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        refreshToken.UpdatedAt = DateTime.UtcNow;
        _context.RefreshTokens.Update(refreshToken);

        await _context.SaveChangesAsync();
    }
    public async Task RevokeAllRefreshTokensAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId== userId && !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync();
    }
}
