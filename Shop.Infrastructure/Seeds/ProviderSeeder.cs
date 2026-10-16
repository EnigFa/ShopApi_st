using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Seeds;

public class ProviderSeeder(ShopDbContext db)
{
    private static readonly string[] ProviderNames = ["google", "fb", "apple"];

    public async Task SeedAsync()
    {
        var existing = await db.Providers
            .Select(provider => provider.Name)
            .ToListAsync();

        var missing = ProviderNames
            .Where(name => !existing.Contains(name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (missing.Count == 0)
            return;

        foreach (var name in missing)
            db.Providers.Add(new Provider { Name = name });

        await db.SaveChangesAsync();
    }
}
