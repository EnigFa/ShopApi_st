using AutoMapper;
using MediatR;
using Shop.Application.Interfaces.Repository;
namespace Shop.Application.Commands.Product;

public class CreateProductHandler(IProductRepository repository, IMapper mapper)
    : IRequestHandler<CreateProductCommand, int?>
{
    public async Task<int?> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = mapper.Map<Shop.Domain.Models.Product>(request.Dto);

        product.Images = request.Dto.Images
            .Select(url => new Shop.Domain.Models.ProductImage { Url = url })
            .ToList();

        return await repository.CreateProductAsync(product);
    }
}
