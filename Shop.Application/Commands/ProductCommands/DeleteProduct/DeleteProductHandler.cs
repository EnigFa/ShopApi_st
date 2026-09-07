using MediatR;
using Shop.Application.Interfaces.Repository;

namespace Shop.Application.Commands.ProductCommands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, int?>
{
    private readonly IProductRepository _repository;

    public DeleteProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<int?> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var deleted = await _repository.DeleteProductAsync(request.Id);

        if (!deleted)
        {
            return null;
        }

        return request.Id;
    }
}
