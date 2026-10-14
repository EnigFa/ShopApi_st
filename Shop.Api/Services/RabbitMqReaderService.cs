using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shop.Application.DTOs.UserDTOs;
using Shop.Domain.Models;
using Shop.Infrastructure.Configuration;
using System.Text;
using System.Text.Json;
namespace Shop.Api.Services;

/// <summary>
/// Робота класа запускається у фоновому режимі, всі логі пишутся в 1 консоль
/// </summary>
public class RabbitMqReaderService : BackgroundService
{
    private readonly ILogger<RabbitMqReaderService> _logger;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private IConnection? _connection;
    private IChannel? _channel;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    public RabbitMqReaderService(
        ILogger<RabbitMqReaderService> logger,
        IOptions<RabbitMqSettings> options)
    {
        _logger = logger;
        _rabbitMqSettings = options.Value;
    }
    
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSettings.Host,
            Port = _rabbitMqSettings.Port
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (sender, e) =>
                {
                    var body = e.Body.ToArray();

                    var json = Encoding.UTF8.GetString(body);

                    var message = JsonSerializer.Deserialize<UserCreateDTO>(json);

                    if (message == null)
                        return;

                    _logger.LogInformation(
                        "Email: {Email}",
                        message.Email);

                    _logger.LogInformation(
                        "Password: {Password}",
                        message.Password);

                    await Task.CompletedTask;
                };

                await _channel.BasicConsumeAsync(
                    queue: "Users",
                    autoAck: true,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation(
                    "RabbitMQ Reader started. Waiting messages...");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "RabbitMQ недоступний ({Host}:{Port}). Повтор через 5 секунд.",
                    _rabbitMqSettings.Host,
                    _rabbitMqSettings.Port);

                await CloseQuietlyAsync();

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task CloseQuietlyAsync()
    {
        try
        {
            if (_channel != null)
                await _channel.CloseAsync();
        }
        catch (Exception exception)
        {
            _logger.LogDebug(exception, "RabbitMQ channel close failed.");
        }

        try
        {
            if (_connection != null)
                await _connection.CloseAsync();
        }
        catch (Exception exception)
        {
            _logger.LogDebug(exception, "RabbitMQ connection close failed.");
        }

        _channel = null;
        _connection = null;
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "RabbitMQ Reader stopping...");

        if (_channel != null)
            await _channel.CloseAsync();

        if (_connection != null)
            await _connection.CloseAsync();

        await base.StopAsync(cancellationToken);
    }
}