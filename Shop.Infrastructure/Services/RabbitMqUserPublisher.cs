using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System.Text;
using System.Text.Json;

namespace Shop.Infrastructure.Services;

public class RabbitMqUserPublisher(
    IOptions<RabbitMqSettings> options,
    ILogger<RabbitMqUserPublisher> logger) : IUserQueuePublisher, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings = options.Value;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;

    public async Task PublishRegisteredUserAsync(
        Guid id,
        string email,
        string role,
        CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(new
        {
            id,
            email,
            role
        });

        var body = Encoding.UTF8.GetBytes(payload);
        var connection = await GetConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: _settings.UsersQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _settings.UsersQueue,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "Користувача {Email} надіслано в чергу {Queue}",
            email,
            _settings.UsersQueue);
    }

    private async Task<IConnection> GetConnectionAsync(
        CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }

        _connectionLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
