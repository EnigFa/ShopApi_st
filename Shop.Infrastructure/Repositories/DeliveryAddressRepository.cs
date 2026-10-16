using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class DeliveryAddressRepository(ShopDbContext context) : IDeliveryAddressRepository
{
    public Task<List<DeliveryAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return context.DeliveryAddresses
            .AsNoTracking()
            .Where(address => address.UserId == userId)
            .OrderByDescending(address => address.IsDefault)
            .ThenByDescending(address => address.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasAnyAsync(Guid userId, CancellationToken cancellationToken)
    {
        return context.DeliveryAddresses
            .AnyAsync(address => address.UserId == userId, cancellationToken);
    }

    public async Task ClearDefaultAsync(Guid userId, CancellationToken cancellationToken)
    {
        var defaults = await context.DeliveryAddresses
            .Where(address => address.UserId == userId && address.IsDefault)
            .ToListAsync(cancellationToken);

        foreach (var address in defaults)
            address.IsDefault = false;
    }

    public async Task AddAsync(DeliveryAddress address, CancellationToken cancellationToken)
    {
        await context.DeliveryAddresses.AddAsync(address, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid userId, int id, CancellationToken cancellationToken)
    {
        var address = await context.DeliveryAddresses
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == id, cancellationToken);

        if (address == null)
            return false;

        if (address.IsDefault)
        {
            var next = await context.DeliveryAddresses
                .Where(item => item.UserId == userId && item.Id != id)
                .OrderByDescending(item => item.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (next != null)
                next.IsDefault = true;
        }

        context.DeliveryAddresses.Remove(address);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
