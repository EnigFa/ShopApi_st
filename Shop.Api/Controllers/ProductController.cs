using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Exceptions.HandlerExceptions;
using Shop.Api.Interfaces;
using Shop.Api.Requests.Products;
using Shop.Application.Commands.Product;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.Product;
using Shop.Domain.Models;

namespace Shop.Api.Controllers;
//URL - Uniform Resource Locator - текстовий рядок, який вказує
//на місце розташування ресурса

/// <summary>
/// Контролер по роботі з продуктами
/// </summary>
/// <param name="_productService"></param>
/// <param name="_imageService"></param>
/// <param name="_configuration"></param>
/// <param name="_mediator"></param>
[ApiController]
[Route("api/v1/[controller]")]
//[LogActionFilter]
public class ProductController(IProductService _productService, 
    IImageService _imageService, 
    IConfiguration _configuration, 
    IMediator _mediator) : ControllerBase
{
    /// <summary>Створити новий продукт разом із фотографіями</summary>
    /// <param name="dto">Дані продукту та файли зображень</param>
    /// <returns>Ідентифікатор створеного продукту</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductCreateRequest dto)
    {
        int maxImages = _configuration.GetValue<int>("ProductSettings:MaxImages");
        if (dto.ImagesFiles.Count > maxImages) return BadRequest($"Maximum allowed images: {maxImages}");

        dto.Images = [];
        string directory = _configuration["DirnameForFiles:Products"]
               ?? throw new InvalidOperationException("Не налаштовано каталог для збереження зображень продуктів.");

        foreach (var file in dto.ImagesFiles)
        {
           

            string? url = await _imageService.SaveFileAsync(file, directory);

            if (!string.IsNullOrEmpty(url)) dto.Images.Add(url);
        }

        var productDto = new ProductCreateDTO
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description ?? string.Empty,
            StockQty = dto.StockQty,
            IsActive = dto.IsActive,
            CategoryId = dto.CategoryId,
            Images = dto.Images
        };

        int? id = await _mediator.Send(new CreateProductCommand(productDto));

        return Ok($"Product created {id}");
    }

    /// <summary>Отримати список усіх продуктів</summary>
    /// <returns>Список продуктів</returns>
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetProductsAsync();
        return Ok(products);
    }



    /// <summary>Отримати продукт за ID</summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>Продукт або NotFound</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        //var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            throw new NotFoundException($"Продукт з ID {id} не знайдений");

        return Ok(product);
    }

    /// <summary>
    /// Отримання продукта за slug
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetProductBySlug(string slug)
    {
        return Ok();
    }

    /// <summary>
    /// Отримання товарів за категорією
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("by-category/{id}")]
    public async Task<IActionResult> GetProductByCategoryId([FromRoute]int id)
    {
        var products = await _productService.GetAllProductsByIdCategoryAsync(id);
        if (products == null) return NotFound("Products not found");

        return Ok(products);
    }

    /// <summary>Оновити існуючий продукт</summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <param name="dto">Нові дані продукту</param>
    /// <returns>Результат оновлення</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateRequest dto)
    {
        if (dto.ImagesFiles != null && dto.ImagesFiles.Count > 0)
        {
            dto.Images = [];

            string directory = _configuration["DirnameForFiles:Products"]
               ?? throw new InvalidOperationException("Не налаштовано каталог для збереження зображень продуктів.");

            foreach (var file in dto.ImagesFiles)
            {
                string? url = await _imageService.SaveFileAsync(file, directory);
                if (!string.IsNullOrEmpty(url)) dto.Images.Add(url);
            }
        }

        bool updated = await _productService.UpdateProductAsync(id, dto);
        if (!updated) return NotFound("Product not found");

        return Ok("Product updated");
    }

    /// <summary>Видалити продукт</summary>
    /// <param name="id">Ідентифікатор продукту</param>
    /// <returns>Результат видалення</returns>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        bool deleted = await _productService.DeleteProductAsync(id);
        if (!deleted) return NotFound("Product not found");

        return NoContent();
    }
}
