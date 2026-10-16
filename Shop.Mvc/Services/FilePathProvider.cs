using Shop.Application.Interfaces.Configurations;

namespace Shop.Mvc.Services;

public class FilePathProvider(IConfiguration configuration) : IFilePathProvider
{
    public string Categories =>
        configuration["DirnameForFiles:Categories"]
        ?? throw new InvalidOperationException("Не налаштовано каталог для зображень категорій.");

    public string Products =>
        configuration["DirnameForFiles:Products"]
        ?? throw new InvalidOperationException("Не налаштовано каталог для зображень продуктів.");
}
