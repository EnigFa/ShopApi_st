using MediatR;

namespace Shop.Application.Commands.ProductCommands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest<int?>;
