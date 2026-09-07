using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Shop.Application.Interfaces.Services;
using System.Text.Json;

namespace Shop.Infrastructure.Services;

public class RedisCacheService(
    IDistributedCache cache,
    ILogger<RedisCacheService> logger) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var json = await cache.GetStringAsync(key);

            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis GET failed for key {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);

            await cache.SetStringAsync(
                key,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl
                });
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis SET failed for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis REMOVE failed for key {Key}", key);
        }
    }
}
