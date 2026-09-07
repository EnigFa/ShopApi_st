using AutoMapper;
using MediatR;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;

namespace Shop.Application.Queries.ProductQueries.GetAllProducts;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, ICollection<ProductReadDTO>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetAllProductsHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ICollection<ProductReadDTO>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllProductsAsync();
        return _mapper.Map<ICollection<ProductReadDTO>>(products);
    }
}
