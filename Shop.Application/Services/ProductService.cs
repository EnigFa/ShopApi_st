using AutoMapper;
using Microsoft.Extensions.Logging;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class ProductService(
    IProductRepository repository,
    ICacheService cache,
    IMapper mapper,
    ILogger<ProductService> logger) : IProductService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public async Task<ProductReadDTO?> GetProductByIdAsync(int id)
    {
        var key = ProductKey(id);
        var cached = await cache.GetAsync<ProductReadDTO>(key);

        if (cached != null)
        {
            logger.LogInformation("Product cache HIT ({Key})", key);
            return cached;
        }

        logger.LogInformation("Product cache MISS ({Key}), loading from DB", key);

        var product = await repository.GetProductByIdAsync(id);

        if (product == null)
        {
            return null;
        }

        var dto = mapper.Map<ProductReadDTO>(product);
        await cache.SetAsync(key, dto, CacheTtl);
        return dto;
    }

    public async Task<List<ProductReadDTO>> GetAllProductsAsync()
    {
        const string key = "products:all";
        var cached = await cache.GetAsync<List<ProductReadDTO>>(key);

        if (cached != null)
        {
            logger.LogInformation("Products cache HIT ({Key})", key);
            return cached;
        }

        logger.LogInformation("Products cache MISS ({Key}), loading from DB", key);

        var products = await repository.GetAllProductsAsync();
        var dtos = mapper.Map<List<ProductReadDTO>>(products);
        await cache.SetAsync(key, dtos, CacheTtl);
        return dtos;
    }

    public async Task<ProductReadDTO> CreateProductAsync(ProductCreateDTO dto)
    {
        var product = mapper.Map<Product>(dto);
        var created = await repository.AddProductAsync(product);
        var readDto = mapper.Map<ProductReadDTO>(created);

        await cache.RemoveAsync("products:all");
        await cache.RemoveAsync(ProductKey(created.Id));
        await cache.RemoveAsync(CategoryKey(created.CategoryId));

        return readDto;
    }

    private static string ProductKey(int id) => $"products:{id}";

    private static string CategoryKey(int id) => $"categories:{id}";
}
