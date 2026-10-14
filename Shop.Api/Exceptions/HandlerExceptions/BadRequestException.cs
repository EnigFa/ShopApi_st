namespace Shop.Api.Exceptions.HandlerExceptions;

/// <summary>Некоректний запит: бізнес-правило не виконано.</summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}
