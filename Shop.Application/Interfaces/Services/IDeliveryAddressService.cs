using Shop.Application.DTOs.DeliveryAddressDTOs;

namespace Shop.Application.Interfaces.Services;

public interface IDeliveryAddressService
{
    Task<List<DeliveryAddressReadDTO>?> GetMyAddressesAsync(string email, CancellationToken cancellationToken);

    Task<DeliveryAddressReadDTO?> AddAsync(string email, DeliveryAddressCreateDTO dto, CancellationToken cancellationToken);

    Task<bool?> DeleteAsync(string email, int id, CancellationToken cancellationToken);
}
