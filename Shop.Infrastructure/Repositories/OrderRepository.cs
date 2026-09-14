using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class OrderRepository(ShopDbContext context) : IOrderRepository
{
    public async Task<User?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public async Task<List<Product>> GetTrackedProductsAsync(
        IEnumerable<int> productIds,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds.Distinct().ToList();

        return await context.Products
            .Where(product => ids.Contains(product.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddOrderAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        await context.Orders.AddAsync(order, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
