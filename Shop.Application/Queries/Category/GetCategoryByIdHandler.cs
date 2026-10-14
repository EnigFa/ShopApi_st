using AutoMapper;
using MediatR;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Configurations;
using Shop.Application.Interfaces.Repository;

namespace Shop.Application.Queries.Category;

public class GetCategoryByIdHandler(
    ICategoryRepository repository,
    IMapper mapper,
    IFilePathProvider filePathProvider)
    : IRequestHandler<GetCategoryByIdQuery, CategoryReadDTO?>
{
    public async Task<CategoryReadDTO?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await repository.GetCategoryByIdAsync(request.Id);
        if (category == null)
            return null;

        var dto = mapper.Map<CategoryReadDTO>(category);
        CategoryImageUrl.Fix(dto, filePathProvider.Categories);
        return dto;
    }
}
