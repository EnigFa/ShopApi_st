using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface ICategoryRepository
{
    Task<int?> AddCategoryAsync(Category category);

    Task<Category?> GetCategoryByIdAsync(int id);

    Task<Category?> GetCategoryBySlugAsync(string slug);

    Task<List<Category>?> GetAllCategoriesAsync(CancellationToken ct);

    Task<List<Category>?> GetCategoriesByParentIdAsync(int id);
}
