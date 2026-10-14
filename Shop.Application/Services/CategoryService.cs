using AutoMapper;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Configurations;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;


public class CategoryService(ICategoryRepository _repository, IMapper _mapper, ICachingService _cachingService,IFilePathProvider _filePathProvider) : ICategoryService
{
    private const string CategoriesCacheKey = "Categories";

    /// <summary>
    /// Метод формує "правильний" шлях до картинки
    /// </summary>
    /// <param name="dto"></param>
    private void FixedImageForCategory(CategoryReadDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Url))
        {
            dto.Url = string.Empty;
            return;
        }

        var url = dto.Url.Replace('\\', '/').Trim();

        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith('/'))
        {
            dto.Url = url;
            return;
        }

        var folder = _filePathProvider.Categories.Trim('/');

        dto.Url = url.StartsWith($"{folder}/", StringComparison.OrdinalIgnoreCase)
            ? $"/{url.TrimStart('/')}"
            : $"/{folder}/{url.TrimStart('/')}";
    }

    public async Task<int?> CreateCategoryAsync(CategoryCreateDTO dto)
    {
        var category = _mapper.Map<Category>(dto);
        var id = await _repository.AddCategoryAsync(category);
        await TryRemoveCacheAsync(CategoriesCacheKey);
        return id;
    }

    public CategoryReadDTO? GetCategoryById(int id)
    {
        return GetCategoryByIdAsync(id).GetAwaiter().GetResult();
    }

    public async Task<CategoryReadDTO?> GetCategoryByIdAsync(int id)
    {
        CategoryReadDTO? dto = null;
        var category = await _repository.GetCategoryByIdAsync(id);
        if (category != null)
        {
            dto = _mapper.Map<CategoryReadDTO>(category);
            FixedImageForCategory(dto);
        }
        return dto;
    }
    public async Task<List<CategoryReadDTO>?> GetAllCategoriesAsync(CancellationToken ct)
    {
        var cache = await TryGetCacheAsync<List<CategoryReadDTO>>(CategoriesCacheKey);
        if (cache == null)
        {
            Console.WriteLine("Reading from DB...");
            List<Category>? categories = await _repository.GetAllCategoriesAsync(ct);

            if (categories != null && categories.Count > 0)
            {
                cache = _mapper.Map<List<CategoryReadDTO>>(categories);
                cache.ForEach(dto => FixedImageForCategory(dto));
                await TrySetCacheAsync(CategoriesCacheKey, cache, null);
            }
        }

        return cache;

        //List<Category>? categories = await _repository.GetAllCategoriesAsync();
        //List<CategoryReadDTO>? cat = null;
        //if (categories != null && categories.Count > 0)
        //{
        //    cat = _mapper.Map<List<CategoryReadDTO>>(categories);
        //    cat.ForEach(dto => FixedImageForCategory(dto));

        //}
        //return cat;

    }

    public async Task<CategoryReadDTO?> GetCategoryBySlugAsync(string slug)
    {
        CategoryReadDTO? dto = null;
        var category = await _repository.GetCategoryBySlugAsync(slug);
        if (category != null)
        {
            dto = _mapper.Map<CategoryReadDTO>(category);
            FixedImageForCategory(dto);
        }
        return dto;
    }

    public async Task<List<CategoryReadDTO>?> GetAllCategoriesByParentIdAsync(int id)
    {
        var categories = await _repository.GetCategoriesByParentIdAsync(id);
        if (categories != null)
        {
            var dtos = _mapper.Map<List<CategoryReadDTO>>(categories);
            dtos.ForEach(dto => FixedImageForCategory(dto));
            return dtos;
        }
        return null;
    }

    private async Task<T?> TryGetCacheAsync<T>(string key)
    {
        try
        {
            return await _cachingService.GetAsync<T>(key);
        }
        catch
        {
            return default;
        }
    }

    private async Task TrySetCacheAsync<T>(string key, T value, TimeSpan? expiration)
    {
        try
        {
            await _cachingService.SetAsync(key, value, expiration);
        }
        catch
        {
            // Каталог має відкриватися навіть без Redis.
        }
    }

    private async Task TryRemoveCacheAsync(string key)
    {
        try
        {
            await _cachingService.RemoveAsync(key);
        }
        catch
        {
            // Нова категорія вже збережена. Застарілий кеш оновиться сам.
        }
    }
}
