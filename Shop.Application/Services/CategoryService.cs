using AutoMapper;
using Microsoft.Extensions.Logging;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class CategoryService(
    ICategoryRepository repository,
    ICacheService cache,
    IMapper mapper,
    ILogger<CategoryService> logger) : ICategoryService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public async Task<int?> CreateCategoryAsync(CategoryCreateDTO dto)
    {
        var category = mapper.Map<Category>(dto);
        var id = await repository.AddCategoryAsync(category);

        if (id.HasValue)
        {
            await cache.RemoveAsync(CategoryKey(id.Value));
        }

        return id;
    }

    public async Task<CategoryReadDTO?> GetCategoryByIdAsync(int id)
    {
        var key = CategoryKey(id);
        var cached = await cache.GetAsync<CategoryReadDTO>(key);

        if (cached != null)
        {
            logger.LogInformation("Category cache HIT ({Key})", key);
            return cached;
        }

        logger.LogInformation("Category cache MISS ({Key}), loading from DB", key);

        var category = await repository.GetCategoryByIdAsync(id);

        if (category == null)
        {
            return null;
        }

        var dto = mapper.Map<CategoryReadDTO>(category);
        await cache.SetAsync(key, dto, CacheTtl);
        return dto;
    }

    public async Task<List<CategoryReadDTO>?> GetAllCategoriesAsync()
    {
        List<Category>? categories = await repository.GetAllCategoriesAsync();
        List<CategoryReadDTO>? dtos = null;
        if (categories != null && categories.Count > 0)
        {
            dtos = mapper.Map<List<CategoryReadDTO>>(categories);
        }
        return dtos;
    }

    private static string CategoryKey(int id) => $"categories:{id}";
}
