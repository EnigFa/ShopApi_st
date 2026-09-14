using MediatR;

namespace Shop.Application.Commands.ProductCommands.DeactivateProduct;

public record DeactivateProductCommand(int Id) : IRequest<int?>;
