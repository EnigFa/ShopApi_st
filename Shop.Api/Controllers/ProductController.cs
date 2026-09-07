using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Api.Interfaces;
using Shop.Domain.Models;

namespace Shop.Api.Controllers;
//URL - Uniform Resource Locator - текстовий рядок, який вказує
//на місце розташування ресурса

[ApiController]
[Route("api/[controller]")]
[LogActionFilter]
public class ProductController(IProductService _productService) : ControllerBase
{
    [HttpGet]
    public async Task<List<Product>> GetProducts()
    {
        return await _productService.GetAllProductsAsync();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> AddNewProduct([FromBody] Product product)
    {
        await _productService.AddProductAsync(product);
        return Created();
    }
}
