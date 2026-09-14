using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
[LogActionFilter]
public class OrderController(IOrderQueuePublisher orderQueuePublisher) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] OrderCreateDTO dto,
        CancellationToken cancellationToken)
    {
        if (dto.UserId == Guid.Empty)
        {
            return BadRequest("Потрібен userId");
        }

        if (dto.Items is null || dto.Items.Count == 0)
        {
            return BadRequest("Замовлення повинно містити хоча б один товар");
        }

        if (dto.Items.Any(item => item.ProductId <= 0 || item.Count <= 0))
        {
            return BadRequest("Кожна позиція повинна мати коректний productId і count > 0");
        }

        await orderQueuePublisher.PublishOrderAsync(dto, cancellationToken);

        return Accepted(new
        {
            message = "Замовлення прийнято в обробку"
        });
    }
}
