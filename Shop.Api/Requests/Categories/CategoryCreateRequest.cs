using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Api.Requests.Categories;

/// <summary>
/// ..
/// </summary>
public class CategoryCreateRequest:CategoryCreateDTO
{
    /// <summary>
    /// 
    /// </summary>
    public IFormFile? Image { get; set; }
}
