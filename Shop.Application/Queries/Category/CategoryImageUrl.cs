using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Queries.Category;

internal static class CategoryImageUrl
{
    public static void Fix(CategoryReadDTO dto, string categoriesFolder)
    {
        if (string.IsNullOrWhiteSpace(dto.Url))
        {
            dto.Url = string.Empty;
            return;
        }

        var url = dto.Url.Replace('\\', '/').Trim();

        if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
            url.StartsWith('/'))
        {
            dto.Url = url;
            return;
        }

        var folder = categoriesFolder.Trim('/');

        dto.Url = url.StartsWith($"{folder}/", StringComparison.OrdinalIgnoreCase)
            ? $"/{url.TrimStart('/')}"
            : $"/{folder}/{url.TrimStart('/')}";
    }
}
