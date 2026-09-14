using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System.Text;
using System.Text.Json;

namespace Shop.Infrastructure.Services;

public class RabbitMqOrderPublisher(
    IOptions<RabbitMqSettings> options,
    ILogger<RabbitMqOrderPublisher> logger) : IOrderQueuePublisher, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings = options.Value;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection? _connection;

    public async Task PublishOrderAsync(
        OrderCreateDTO order,
        CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(order, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var body = Encoding.UTF8.GetBytes(payload);
        var connection = await GetConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: _settings.OrdersQueue,
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
            routingKey: _settings.OrdersQueue,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        logger.LogInformation(
            "Замовлення користувача {UserId} надіслано в чергу {Queue}",
            order.UserId,
            _settings.OrdersQueue);
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
