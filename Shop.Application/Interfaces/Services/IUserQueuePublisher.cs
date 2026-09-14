namespace Shop.Application.Interfaces.Services;

public interface IUserQueuePublisher
{
    Task PublishRegisteredUserAsync(
        Guid id,
        string email,
        string role,
        CancellationToken cancellationToken = default);
}
