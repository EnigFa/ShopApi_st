using Shop.Application.Interfaces.Services;

namespace Shop.Application.Services;

public class NoOpQueueService : IQueueService
{
    public Task PublishAsync<T>(string queue, T message)
    {
        return Task.CompletedTask;
    }
}
