using Shop.Application.DTOs.OrderDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IOrderProcessingService
{
    Task ProcessAsync(OrderCreateDTO message, CancellationToken cancellationToken = default);
}
