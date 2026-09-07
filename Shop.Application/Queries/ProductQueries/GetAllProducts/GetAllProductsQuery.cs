using MediatR;
using Shop.Application.DTOs.ProductDTOs;

namespace Shop.Application.Queries.ProductQueries.GetAllProducts;

public record GetAllProductsQuery() : IRequest<ICollection<ProductReadDTO>>;
