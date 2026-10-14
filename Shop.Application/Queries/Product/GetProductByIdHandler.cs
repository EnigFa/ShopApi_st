using AutoMapper;
using MediatR;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Queries.Product;

public class GetProductByIdHandler(IProductRepository _productRepository, IMapper _mapper) 
    : IRequestHandler<GetProductByIdQuery, ProductReadDTO?>
{
    public async Task<ProductReadDTO?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(request.id);
        if (product == null)
            return null;
        return _mapper.Map<ProductReadDTO>(product);
    }
}
