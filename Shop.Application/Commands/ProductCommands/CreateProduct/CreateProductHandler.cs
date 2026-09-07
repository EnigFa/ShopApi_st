using MediatR;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;

namespace Shop.Application.Commands.ProductCommands.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, int?>
{
    private readonly IProductRepository _repository;

    public CreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<int?> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQty = request.StockQty,
            IsActive = request.IsActive,
            CategoryId = request.CategoryId
        };

        var result = await _repository.AddProductAsync(product);
        return result.Id;
    }
}
