using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Application.Commands.ProductCommands.CreateProduct;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.ProductQueries.GetAllProducts;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
[LogActionFilter]
public class ProductController(IMediator mediator, IProductService productService) : ControllerBase
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

    [HttpPost]
    public async Task<IActionResult> AddNewProduct(
        [FromBody] ProductCreateDTO dto,
        CancellationToken cancellationToken)
    {
        var id = await mediator.Send(
            new CreateProductCommand(
                dto.Name,
                dto.Description,
                dto.Price,
                dto.StockQty,
                dto.IsActive,
                dto.CategoryId),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetProductById),
            new { id },
            id);
    }
}
