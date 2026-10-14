using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Exceptions.HandlerExceptions;
using Shop.Api.Interfaces;
using Shop.Api.Requests.Categories;
using Shop.Application.Commands.Category;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.Category;
using Shop.Application.Validators.Category;

namespace Shop.Api.Controllers;


[ApiController]
[Route("api/v1/[controller]")] //https://ip:port/api/v1
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IImageService _imageService;
    private readonly IConfiguration _configuration;
    private readonly IValidator<CategoryCreateDTO> _validator;
    private readonly IMediator _mediator;

    public CategoryController(
        ICategoryService categoryService,
        IImageService imageService = null!,
        IConfiguration configuration = null!,
        IValidator<CategoryCreateDTO> validator = null!,
        IMediator mediator = null!)
    {
        _categoryService = categoryService;
        _imageService = imageService;
        _configuration = configuration;
        _validator = validator;
        _mediator = mediator;
    }

    //[Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateRequest dto)
    {
        //var createBookDto = new CategoryCreateDTO
        //{
        //    Name = dto.Name,
        //    Slug = dto.Slug,
        //    ParentId = dto.ParentId,
        //    Url = dto.Url
        //};
        //var result = await _validator.ValidateAsync(createBookDto);
        //if (!result.IsValid)
        //{
        //    return BadRequest(result.Errors);
        //}
        if (dto.Image != null)
        {
            var categoriesDirectory = _configuration["DirnameForFiles:Categories"]
                ?? throw new InvalidOperationException("Не налаштовано каталог для зображень категорій.");

            dto.Url = await _imageService.SaveFileAsync(dto.Image, categoriesDirectory);
        }

        var createDto = new CategoryCreateDTO
        {
            Name = dto.Name,
            Url = dto.Url ?? string.Empty,
            Slug = dto.Slug,
            ParentId = dto.ParentId,
        };

        if (_validator is not null)
        {
            var validationResult = await _validator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray());

                throw new ValidationAppException(errors);
            }
        }

        var id = await _mediator.Send(new CreateCategoryCommand(createDto));

        return CreatedAtAction(
                    nameof(GetCategoryById), // назва методу
                    new { id },              // параметри маршруту
                    new { id });             // тіло відповіді
    }



    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var dto = await _mediator.Send(new GetCategoryByIdQuery(id));

        if (dto == null)
            return NotFound();

        return Ok(dto);
    }

    //[Authorize]

    /// <summary>
    /// Отримання категорії за slug-ом
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    [HttpGet("{slug}")]

    public async Task<ActionResult<CategoryReadDTO>> GetCategoryBySlug(string slug)
    {
        var dto = await _mediator.Send(new GetCategoryBySlugQuery(slug));

        if (dto == null)
            throw new NotFoundException($"Категорія зі slug {slug} не знайдена");

        return Ok(dto);
    }

    //[Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllCategories(CancellationToken ct)
    {
        List<CategoryReadDTO>? categories = await _categoryService.GetAllCategoriesAsync(ct);
        if(categories== null || categories.Count == 0)
        {
            return NotFound();
        }
        return Ok(categories);
    }

    [HttpGet("sub/{id:int}")]
    public async Task<IActionResult> GetAllCategoriesByParentId(int id)
    {
        List<CategoryReadDTO>? categories = await _categoryService.GetAllCategoriesByParentIdAsync(id);
        if (categories == null || categories.Count == 0)
        {
            return NotFound();
        }
        return Ok(categories);
    }

}
