using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqReader;
using System.Text;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var settings = configuration
    .GetSection("RabbitMQ")
    .Get<RabbitMqSettings>()
    ?? throw new InvalidOperationException("Секція RabbitMQ відсутня в appsettings.json");

var factory = new ConnectionFactory
{
    HostName = settings.HostName,
    Port = settings.Port,
    UserName = settings.UserName,
    Password = settings.Password
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: settings.UsersQueue,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (_, ea) =>
{
    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Прочитано з черги {settings.UsersQueue}:");
    Console.WriteLine(json);
    Console.WriteLine();

    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(
    queue: settings.UsersQueue,
    autoAck: false,
    consumer: consumer);

Console.WriteLine(
    $"RabbitMqReader слухає чергу '{settings.UsersQueue}' на {settings.HostName}:{settings.Port}");
Console.WriteLine("Зареєструйте користувача через API. Натисніть Enter, щоб вийти.");
Console.ReadLine();
