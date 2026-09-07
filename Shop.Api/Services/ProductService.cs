using Shop.Api.Interfaces;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Api.Services;

public class ProductService(
    ICacheService cache,
    ILogger<ProductService> logger) : IProductService
{
    private const string AllProductsKey = "products:all";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly List<Product> _products =
    [
        new Product { Id = 1, Name = "Milk", Price = 40.9m, IsActive = true },
        new Product { Id = 2, Name = "Bread", Price = 30.5m, IsActive = true }
    ];

    private readonly object _lock = new();

    public async Task<List<Product>> GetAllProductsAsync()
    {
        var cached = await cache.GetAsync<List<ProductCacheDTO>>(AllProductsKey);

        if (cached != null)
        {
            logger.LogInformation("Products cache HIT ({Key})", AllProductsKey);
            return cached.Select(ToProduct).ToList();
        }

        logger.LogInformation("Products cache MISS ({Key})", AllProductsKey);

        List<Product> products;
        lock (_lock)
        {
            products = _products.ToList();
        }

        await cache.SetAsync(
            AllProductsKey,
            products.Select(ToCacheDto).ToList(),
            CacheTtl);

        return products;
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var key = ProductKey(id);
        var cached = await cache.GetAsync<ProductCacheDTO>(key);

        if (cached != null)
        {
            logger.LogInformation("Product cache HIT ({Key})", key);
            return ToProduct(cached);
        }

        logger.LogInformation("Product cache MISS ({Key})", key);

        Product? product;
        lock (_lock)
        {
            product = _products.FirstOrDefault(item => item.Id == id);
        }

        if (product == null)
        {
            return null;
        }

        await cache.SetAsync(key, ToCacheDto(product), CacheTtl);
        return product;
    }

    public async Task AddProductAsync(Product product)
    {
        lock (_lock)
        {
            if (product.Id == 0)
            {
                product.Id = _products.Count == 0
                    ? 1
                    : _products.Max(item => item.Id) + 1;
            }

            _products.Add(product);
        }

        await cache.RemoveAsync(AllProductsKey);
        await cache.RemoveAsync(ProductKey(product.Id));
        logger.LogInformation("Products cache invalidated after add");
    }

    private static string ProductKey(int id) => $"products:{id}";

    private static ProductCacheDTO ToCacheDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        StockQty = product.StockQty,
        IsActive = product.IsActive,
        CategoryId = product.CategoryId
    };

    private static Product ToProduct(ProductCacheDTO dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        StockQty = dto.StockQty,
        IsActive = dto.IsActive,
        CategoryId = dto.CategoryId
    };
}
