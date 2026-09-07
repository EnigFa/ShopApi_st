using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface IProductRepository
{
    Task<Product?> GetProductByIdAsync(int id);

    Task<List<Product>> GetAllProductsAsync();

    Task<Product> AddProductAsync(Product product);

    Task<bool> DeleteProductAsync(int id);
}
