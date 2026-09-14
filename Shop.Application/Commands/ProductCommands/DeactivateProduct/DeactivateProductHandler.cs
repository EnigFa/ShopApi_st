using MediatR;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;

namespace Shop.Application.Commands.ProductCommands.DeactivateProduct;

public class DeactivateProductHandler : IRequestHandler<DeactivateProductCommand, int?>
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;

    public DeactivateProductHandler(IProductRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<int?> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        var deactivated = await _repository.DeactivateProductAsync(request.Id);

        if (!deactivated)
        {
            return null;
        }

        await _cache.RemoveAsync("products:all");
        await _cache.RemoveAsync($"products:{request.Id}");

        return request.Id;
    }
}
