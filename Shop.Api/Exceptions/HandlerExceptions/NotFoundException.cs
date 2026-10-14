namespace Shop.Api.Exceptions.HandlerExceptions;

/// <summary>Запитуваний ресурс не знайдено. Обробник перетворює цей виняток на 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
