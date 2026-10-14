using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Api.Requests.Products;

public class ProductCreateRequest:ProductCreateDTO
{
    /// <summary>
    /// Фото продуктів
    /// </summary>
    public ICollection<IFormFile> ImagesFiles { get; set; } = [];
}
