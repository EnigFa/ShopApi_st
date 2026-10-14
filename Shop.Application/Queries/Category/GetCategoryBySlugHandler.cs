using AutoMapper;
using MediatR;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Configurations;
using Shop.Application.Interfaces.Repository;

namespace Shop.Application.Queries.Category;

public class GetCategoryBySlugHandler(
    ICategoryRepository repository,
    IMapper mapper,
    IFilePathProvider filePathProvider)
    : IRequestHandler<GetCategoryBySlugQuery, CategoryReadDTO?>
{
    public async Task<CategoryReadDTO?> Handle(GetCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        var category = await repository.GetCategoryBySlugAsync(request.Slug);
        if (category == null)
            return null;

        var dto = mapper.Map<CategoryReadDTO>(category);
        CategoryImageUrl.Fix(dto, filePathProvider.Categories);
        return dto;
    }
}
