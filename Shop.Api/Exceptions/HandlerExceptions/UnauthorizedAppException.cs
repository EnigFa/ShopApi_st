namespace Shop.Api.Exceptions.HandlerExceptions;

/// <summary>Користувач не автентифікований або передав недійсні облікові дані.</summary>
public class UnauthorizedAppException : Exception
{
    public UnauthorizedAppException(string message) : base(message)
    {
    }
}
