using Shop.Application.DTOs.OrderDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IOrderQueuePublisher
{
    Task PublishOrderAsync(
        OrderCreateDTO order,
        CancellationToken cancellationToken = default);
}
