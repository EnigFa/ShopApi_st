using Microsoft.EntityFrameworkCore;
using Shop.Application.Interfaces.Repository;
using Shop.Domain.Models;
using Shop.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Infrastructure.Repositories;

/// <summary>
/// Репозіторій для роботи з продуктами
/// </summary>
/// <param name="_context">
/// Контекст бази даних, який використовується для доступу до таблиць продуктів.
/// </param>
public class ProductRepository(ShopDbContext _context):IProductRepository
{

    /// <inheritdoc/>
    public async Task<int?> CreateProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);

        await _context.SaveChangesAsync();

        return product.Id;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        return await _context.Products
            .Include(x => x.Images)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(x => x.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product?> GetProductForUpdateAsync(int id)
    {
        return await _context.Products
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return false;

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateProductAsync(Product product)
    {
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Product>?> GetAllProductsByIdCategoryAsync(int id)
    {
       var products = await _context.Products.Include(p=>p.Images)
            .Where(p=>p.CategoryId== id).ToListAsync();
        return products;
    }
}
