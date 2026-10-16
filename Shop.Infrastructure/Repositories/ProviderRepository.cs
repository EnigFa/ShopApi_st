using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class ProviderRepository(ShopDbContext context) : IProviderRepository
{
    public Task<Provider?> GetByNameAsync(string name)
    {
        return context.Providers
            .FirstOrDefaultAsync(provider => provider.Name == name);
    }

    public Task<bool> ExistsForUserAsync(Guid userId, int providerId)
    {
        return context.UserProviders
            .AnyAsync(link => link.UserId == userId && link.ProviderId == providerId);
    }

    public Task<bool> NumberTakenAsync(int providerId, string numberProvider)
    {
        return context.UserProviders
            .AnyAsync(link => link.ProviderId == providerId && link.NumberProvider == numberProvider);
    }

    public async Task AddUserProviderAsync(UserProvider link)
    {
        await context.UserProviders.AddAsync(link);
        await context.SaveChangesAsync();
    }
}
