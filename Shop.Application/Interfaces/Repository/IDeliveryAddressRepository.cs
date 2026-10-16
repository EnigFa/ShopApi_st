using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IDeliveryAddressRepository
{
    Task<List<DeliveryAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> HasAnyAsync(Guid userId, CancellationToken cancellationToken);

    Task ClearDefaultAsync(Guid userId, CancellationToken cancellationToken);

    Task AddAsync(DeliveryAddress address, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid userId, int id, CancellationToken cancellationToken);
}
