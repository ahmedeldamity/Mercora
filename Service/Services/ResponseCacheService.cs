using BlazorEcommerce.Application.Interfaces.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace BlazorEcommerce.Infrastructure.Services;
public class ResponseCacheService(IConnectionMultiplexer connection) : IResponseCacheService
{
    private readonly IDatabase _database = connection.GetDatabase();

    public async Task CacheResponseAsync(string cacheKey, object? response, TimeSpan timeToLive)
    {
        if (response is null) return;

        var serializeOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var responseJson = JsonSerializer.Serialize(response, serializeOptions);

        await _database.StringSetAsync(cacheKey, responseJson, timeToLive);
    }

    public async Task<string?> GetCachedResponseAsync(string cacheKey)
    {
        var cachedResponse = await _database.StringGetAsync(cacheKey);

        if (cachedResponse.IsNullOrEmpty) return null;

        return cachedResponse;
    }

}