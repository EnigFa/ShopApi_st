using MediatR;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Queries.Category;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryReadDTO?>;
