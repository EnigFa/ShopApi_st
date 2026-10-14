using Shop.Application.Interfaces.Configurations;

namespace Shop.Api.Services;

/// <summary>
/// Клас дозволяє прочитати appsettings.json в інших слоях.
/// Написан для формування шляху до малюнків категорій та пролуктів
/// </summary>
/// <param name="_configuration"></param>
public class FilePathProvider(IConfiguration _configuration):IFilePathProvider
{
    private string _dirname = "DirnameForFiles";
    public string Categories =>
        _configuration[$"{_dirname}:Categories"]
        ?? throw new InvalidOperationException();

    public string Products =>
        _configuration[$"{_dirname}:Products"]
        ?? throw new InvalidOperationException();

}
