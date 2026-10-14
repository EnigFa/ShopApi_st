using Shop.Application.Interfaces.Services;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shop.Infrastructure.Services;

public class RedisCachingService:ICachingService
{
    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IDatabase _database;
    public RedisCachingService(IConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
        _database = multiplexer.GetDatabase();
    }

    private bool CanUseCache()
    {
        return _multiplexer.IsConnected;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        if (!CanUseCache())
        {
            return default;
        }

        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default(T?);
        }
        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task RemoveAsync(string key)
    {
        if (!CanUseCache())
        {
            return;
        }

        await _database.KeyDeleteAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? exp)
    {
        if (!CanUseCache())
        {
            return;
        }

        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, (exp == null) ? TimeSpan.FromMinutes(15) : exp.Value);

    }
}
