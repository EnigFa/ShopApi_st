using Shop.Domain.Models;

namespace Shop.Application.Interfaces.Repository;

public interface ICategoryRepository
{
    Task<int?> AddCategoryAsync(Category category);

    Task<Category?> GetCategoryByIdAsync(int id);

    Task<List<Category>?> GetAllCategoriesAsync();
}
