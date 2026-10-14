namespace Shop.Api.Exceptions.HandlerExceptions;

/// <summary>Помилка валідації. Повідомлення по полях лежать у <see cref="Errors"/>.</summary>
public class ValidationAppException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationAppException(Dictionary<string, string[]> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }
}
