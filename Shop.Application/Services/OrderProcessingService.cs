using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Enums;
using Shop.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Shop.Application.Services;

public class OrderProcessingService(
    IOrderRepository orderRepository,
    IEmailService emailService,
    ILogger<OrderProcessingService> logger) : IOrderProcessingService
{
    public async Task ProcessAsync(
        OrderCreateDTO message,
        CancellationToken cancellationToken = default)
    {
        if (message.UserId == Guid.Empty ||
            message.Items is null ||
            message.Items.Count == 0)
        {
            logger.LogWarning("Замовлення відхилено: порожній користувач або список товарів");
            return;
        }

        if (message.Items.Any(item => item.ProductId <= 0 || item.Count <= 0))
        {
            logger.LogWarning("Замовлення відхилено: некоректні позиції");
            return;
        }

        var user = await orderRepository.GetUserByIdAsync(message.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            logger.LogWarning(
                "Замовлення відхилено: користувач {UserId} не знайдений або неактивний",
                message.UserId);
            return;
        }

        var groupedItems = message.Items
            .GroupBy(item => item.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                Count = group.Sum(item => item.Count)
            })
            .ToList();

        var products = await orderRepository.GetTrackedProductsAsync(
            groupedItems.Select(item => item.ProductId),
            cancellationToken);

        var productsById = products.ToDictionary(product => product.Id);

        foreach (var item in groupedItems)
        {
            if (!productsById.TryGetValue(item.ProductId, out var product) ||
                !product.IsActive)
            {
                logger.LogWarning(
                    "Замовлення відхилено: товар {ProductId} не знайдений або неактивний",
                    item.ProductId);
                return;
            }
        }

        var hasEnoughStock = groupedItems.All(item =>
            productsById[item.ProductId].StockQty >= item.Count);

        var order = new Order
        {
            UserId = user.Id,
            Paid = message.Paid,
            Status = hasEnoughStock ? OrderStatus.Confirmed : OrderStatus.Waiting
        };

        foreach (var item in groupedItems)
        {
            var product = productsById[item.ProductId];

            order.Details.Add(new OrderDetail
            {
                ProductId = product.Id,
                Price = product.Price,
                Count = item.Count
            });

            if (hasEnoughStock)
            {
                product.StockQty -= item.Count;
            }
        }

        await orderRepository.AddOrderAsync(order, cancellationToken);

        var lines = order.Details
            .Select(detail => new OrderEmailLineDTO
            {
                ProductName = productsById[detail.ProductId].Name,
                Price = detail.Price,
                Count = detail.Count
            })
            .ToList();

        var total = lines.Sum(line => line.LineTotal);

        if (hasEnoughStock)
        {
            await emailService.SendOrderConfirmationAsync(
                user.Email,
                order.Id,
                lines,
                total);
        }
        else
        {
            await emailService.SendOrderWaitingAsync(
                user.Email,
                order.Id,
                lines,
                total);
        }

        logger.LogInformation(
            "Замовлення {OrderId} збережено зі статусом {Status}",
            order.Id,
            order.Status);
    }
}
