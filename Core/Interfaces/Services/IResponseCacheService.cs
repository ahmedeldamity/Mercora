namespace Core.Interfaces.Services;
public interface IResponseCacheService
{
    Task CacheResponseAsync(string cacheKey, object? response, TimeSpan timeToLive);
    Task<string?> GetCachedResponseAsync(string cacheKey);
}