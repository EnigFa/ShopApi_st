namespace Shop.Api.Middlewares;

public class CancellationTokenHandleMiddleware(RequestDelegate next, ILogger<CancellationTokenHandleMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception _) when(_ is OperationCanceledException or TaskCanceledException)
        {
            logger.LogError("Request cancelled");
        }
    }
}