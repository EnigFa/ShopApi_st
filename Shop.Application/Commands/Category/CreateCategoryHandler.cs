using AutoMapper;
using MediatR;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
namespace Shop.Application.Commands.Category;

public class CreateCategoryHandler(
    ICategoryRepository repository,
    IMapper mapper,
    ICachingService cachingService)
    : IRequestHandler<CreateCategoryCommand, int?>
{
    private const string CategoriesCacheKey = "Categories";

    public async Task<int?> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = mapper.Map<Shop.Domain.Models.Category>(request.Dto);
        var id = await repository.AddCategoryAsync(category);
        await TryRemoveCacheAsync(CategoriesCacheKey);
        return id;
    }

    private async Task TryRemoveCacheAsync(string key)
    {
        try
        {
            await cachingService.RemoveAsync(key);
        }
        catch
        {
            // Нова категорія вже збережена. Застарілий кеш оновиться сам.
        }
    }
}
