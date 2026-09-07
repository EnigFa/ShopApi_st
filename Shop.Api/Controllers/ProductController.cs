using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/products")]
[LogActionFilter]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await productService.GetAllProductsAsync();
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
    public async Task<IActionResult> AddNewProduct([FromBody] ProductCreateDTO dto)
    {
        var product = await productService.CreateProductAsync(dto);

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id },
            product);
    }
}
