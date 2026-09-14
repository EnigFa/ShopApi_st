using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IOrderRepository
{
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<Product>> GetTrackedProductsAsync(
        IEnumerable<int> productIds,
        CancellationToken cancellationToken = default);

    Task AddOrderAsync(Order order, CancellationToken cancellationToken = default);
}
