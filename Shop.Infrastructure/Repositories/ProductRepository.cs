using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;

namespace Shop.Infrastructure.Repositories;

public class ProductRepository(ShopDbContext context) : IProductRepository
{
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id);
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await context.Products
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(item => item.Id == id);

        if (product == null)
        {
            return false;
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return true;
    }
}
