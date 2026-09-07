using MediatR;

namespace Shop.Application.Commands.ProductCommands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int StockQty,
    bool IsActive,
    int CategoryId) : IRequest<int?>;
