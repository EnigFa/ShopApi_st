using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Exceptions.HandlerExceptions;
using Shop.Application.DTOs.DeliveryAddressDTOs;
using Shop.Application.Interfaces.Services;
using System.Security.Claims;

namespace Shop.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/delivery-addresses")]
public class DeliveryAddressController(IDeliveryAddressService deliveryAddressService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<DeliveryAddressReadDTO>>> GetMyAddresses(CancellationToken cancellationToken)
    {
        var email = GetCurrentUserEmail();
        var addresses = await deliveryAddressService.GetMyAddressesAsync(email, cancellationToken);

        if (addresses == null)
            throw new UnauthorizedAppException("Користувача не знайдено");

        return Ok(addresses);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeliveryAddressReadDTO>> GetById(int id, CancellationToken cancellationToken)
    {
        var email = GetCurrentUserEmail();
        var addresses = await deliveryAddressService.GetMyAddressesAsync(email, cancellationToken);

        if (addresses == null)
            throw new UnauthorizedAppException("Користувача не знайдено");

        var address = addresses.FirstOrDefault(item => item.Id == id);
        if (address == null)
            throw new NotFoundException($"Адресу доставки {id} не знайдено");

        return Ok(address);
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryAddressReadDTO>> AddAddress(
        [FromBody] DeliveryAddressCreateDTO dto,
        CancellationToken cancellationToken)
    {
        var email = GetCurrentUserEmail();
        var created = await deliveryAddressService.AddAsync(email, dto, cancellationToken);

        if (created == null)
            throw new UnauthorizedAppException("Користувача не знайдено");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAddress(int id, CancellationToken cancellationToken)
    {
        var email = GetCurrentUserEmail();
        var deleted = await deliveryAddressService.DeleteAsync(email, id, cancellationToken);

        if (deleted == null)
            throw new UnauthorizedAppException("Користувача не знайдено");

        if (deleted == false)
            throw new NotFoundException($"Адресу доставки {id} не знайдено");

        return NoContent();
    }

    private string GetCurrentUserEmail()
    {
        var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email");

        if (string.IsNullOrWhiteSpace(email))
            throw new UnauthorizedAppException("Користувача не визначено");

        return email;
    }
}
