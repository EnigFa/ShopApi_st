using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMqReader;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Application.Services;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
using Shop.Infrastructure.Services;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("Email"));

builder.Services.AddDbContext<ShopDbContext>(options =>
{
    var connectionString = builder.Configuration["ConnectionStrings:SqlServerConnection"]
        ?? throw new InvalidOperationException("ConnectionStrings:SqlServerConnection is missing");

    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderProcessingService, OrderProcessingService>();
builder.Services.AddHostedService<UsersQueueListener>();
builder.Services.AddHostedService<OrdersQueueListener>();

var host = builder.Build();
await host.RunAsync();
