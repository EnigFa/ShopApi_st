using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Application.Commands.ProductCommands.DeactivateProduct;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.ProductQueries.GetAllProducts;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
[LogActionFilter]
public class ProductController(
    IMediator mediator,
    IProductService productService,
    IProductFeedbackService productFeedbackService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var products = await mediator.Send(new GetAllProductsQuery(), cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById([FromRoute] int id)
    {
        var product = await productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateProduct(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var deactivatedId = await mediator.Send(new DeactivateProductCommand(id), cancellationToken);

        if (deactivatedId == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:int}/feedback")]
    public async Task<IActionResult> AddProductFeedback(
        [FromRoute] int id,
        [FromBody] ProductFeedbackCreateDTO dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var feedback = await productFeedbackService.AddAsync(id, dto, cancellationToken);
            return Created($"/api/v1/products/{id}/feedback/{feedback.Id}", feedback);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
