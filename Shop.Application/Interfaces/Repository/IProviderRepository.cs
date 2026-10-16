using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IProviderRepository
{
    Task<Provider?> GetByNameAsync(string name);

    Task<bool> ExistsForUserAsync(Guid userId, int providerId);

    Task<bool> NumberTakenAsync(int providerId, string numberProvider);

    Task AddUserProviderAsync(UserProvider link);
}
