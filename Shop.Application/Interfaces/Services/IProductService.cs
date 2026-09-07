using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IProductService
{
    Task<ProductReadDTO?> GetProductByIdAsync(int id);

    Task<List<ProductReadDTO>> GetAllProductsAsync();

    Task<ProductReadDTO> CreateProductAsync(ProductCreateDTO dto);
}
