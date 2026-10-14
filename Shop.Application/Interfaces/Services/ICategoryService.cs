using Shop.Application.DTOs.CategoryDTOs;


namespace Shop.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<int?> CreateCategoryAsync(CategoryCreateDTO dto);
    CategoryReadDTO? GetCategoryById(int id);
    Task<CategoryReadDTO?> GetCategoryByIdAsync(int id);

    Task<CategoryReadDTO?> GetCategoryBySlugAsync(string slug);
    Task<List<CategoryReadDTO>?> GetAllCategoriesAsync(CancellationToken ct);

    Task<List<CategoryReadDTO>?> GetAllCategoriesByParentIdAsync(int id);

}
