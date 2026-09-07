using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Interfaces;
using Shop.Api.Requests.Categories;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;

namespace Shop.Api.Controllers;


[ApiController]
[Route("api/v1/[controller]")] //https://ip:port/api/v1
public class CategoryController(ICategoryService _categoryService, IImageService _imageService, IConfiguration _configuration) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateRequest dto)
    {
        if (dto.Image != null)
        {
            dto.Url = (await _imageService.SaveFileAsync(dto.Image, _configuration["DirnameForFiles:Categories"])) ?? string.Empty;
        }

        var createDto = new CategoryCreateDTO
        {
            Name = dto.Name,
            Url = dto.Url,
            Slug = dto.Slug,
            ParentId = dto.ParentId,
        };

        var id = await _categoryService.CreateCategoryAsync(createDto);

        return CreatedAtAction(
                    nameof(GetCategoryById), // назва методу
                    new { id },              // параметри маршруту
                    new { id });             // тіло відповіді
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryReadDTO>> GetCategoryById(int id)
    {
        var dto = await _categoryService.GetCategoryByIdAsync(id);

        if (dto == null)
            return NotFound();

        return Ok(dto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        List<CategoryReadDTO>? categories = await _categoryService.GetAllCategoriesAsync();
        if(categories== null || categories.Count == 0)
        {
            return NotFound();
        }
        return Ok(categories);
    }
}
