using Shop.Domain.Models;

namespace Shop.Api.Interfaces;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();

    Task<Product?> GetProductByIdAsync(int id);

    Task AddProductAsync(Product product);
}
