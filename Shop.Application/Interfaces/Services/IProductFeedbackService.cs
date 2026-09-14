using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IProductFeedbackService
{
    Task<ProductFeedbackReadDTO> AddAsync(
        int productId,
        ProductFeedbackCreateDTO dto,
        CancellationToken cancellationToken = default);
}
