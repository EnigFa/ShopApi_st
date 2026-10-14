using Microsoft.Extensions.Options;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Shop.Infrastructure.Services;

public class RabbitMqService : IQueueService
{
    private readonly RabbitMqSettings _rabbitMqSettings;


    public RabbitMqService(IOptions<RabbitMqSettings> options)
    {
        _rabbitMqSettings = options.Value;
    }

    // Метод для відправки повідомлення у чергу
    public async Task PublishAsync<T>(string queue, T message)
    {
        // Створюємо фабрику підключення до RabbitMQ
        var factory = new ConnectionFactory()
        {
            // Host сервера RabbitMQ
            HostName = _rabbitMqSettings.Host,

            // Порт RabbitMQ (зазвичай 5672)
            Port = _rabbitMqSettings.Port
        };

        // Створюємо з'єднання з RabbitMQ сервером
        await using var connection = await factory.CreateConnectionAsync();

        // Створюємо канал (channel) для роботи з чергами
        await using var channel = await connection.CreateChannelAsync();

        // Оголошуємо чергу
        // Якщо черга не існує — вона буде створена
        await channel.QueueDeclareAsync(
            queue: queue,          // назва черги
            durable: true,         // черга зберігається після перезапуску RabbitMQ
            exclusive: false,      // доступна для інших з'єднань
            autoDelete: false,     // не видаляється автоматично
            arguments: null        // додаткові параметри
        );

        // Серіалізуємо повідомлення у JSON
        var json = JsonSerializer.Serialize(message);

        // Перетворюємо JSON у масив байтів
        // RabbitMQ передає повідомлення саме у вигляді байтів
        var body = Encoding.UTF8.GetBytes(json);

        // Властивості повідомлення
        var properties = new BasicProperties
        {
            // Робить повідомлення persistent (зберігається на диску)
            Persistent = true
        };

        // Відправляємо повідомлення у чергу
        await channel.BasicPublishAsync(
             exchange: "",        // стандартний exchange
             routingKey: queue,   // назва черги (routing key)
             mandatory: false,    // якщо черга не знайдена — повідомлення просто ігнорується
             basicProperties: properties,
             body: body           // тіло повідомлення
        );
    }
}
