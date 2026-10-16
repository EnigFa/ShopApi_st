using AutoMapper;
using Shop.Application.DTOs.DeliveryAddressDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class DeliveryAddressService(
    IDeliveryAddressRepository repository,
    IAuthRepository authRepository,
    IMapper mapper) : IDeliveryAddressService
{
    public async Task<List<DeliveryAddressReadDTO>?> GetMyAddressesAsync(string email, CancellationToken cancellationToken)
    {
        var user = await authRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
            return null;

        var addresses = await repository.GetByUserIdAsync(user.Id, cancellationToken);
        return mapper.Map<List<DeliveryAddressReadDTO>>(addresses);
    }

    public async Task<DeliveryAddressReadDTO?> AddAsync(string email, DeliveryAddressCreateDTO dto, CancellationToken cancellationToken)
    {
        var user = await authRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
            return null;

        var address = mapper.Map<DeliveryAddress>(dto);
        address.UserId = user.Id;

        var hasAny = await repository.HasAnyAsync(user.Id, cancellationToken);
        if (!hasAny || dto.IsDefault)
        {
            if (hasAny)
                await repository.ClearDefaultAsync(user.Id, cancellationToken);

            address.IsDefault = true;
        }

        await repository.AddAsync(address, cancellationToken);
        return mapper.Map<DeliveryAddressReadDTO>(address);
    }

    public async Task<bool?> DeleteAsync(string email, int id, CancellationToken cancellationToken)
    {
        var user = await authRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
            return null;

        return await repository.DeleteAsync(user.Id, id, cancellationToken);
    }
}
