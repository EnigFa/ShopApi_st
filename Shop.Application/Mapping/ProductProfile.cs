using AutoMapper;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Domain.Models;

namespace Shop.Application.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductReadDTO>();
        CreateMap<ProductCreateDTO, Product>();
    }
}
