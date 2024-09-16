using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace API.Helpers;
public class CachedAttribute(int timeToLiveInSeconds) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

        var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

        var cachedResponse = await responseCacheService.GetCachedResponseAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedResponse))
        {
            var contentResult = new ContentResult
            {
                Content = cachedResponse,
                ContentType = "application/json",
                StatusCode = 200
            };

            context.Result = contentResult;

            return;
        }

        var executedActionContext = await next.Invoke();

        if (executedActionContext.Result is OkObjectResult { Value: not null } okObjectResult)
        {
            await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(timeToLiveInSeconts));
        }
    }

    private static string GenerateCacheKeyFromRequest(HttpRequest request)
    {
        var keyBuilder = new StringBuilder();

        keyBuilder.Append(request.Path);

        foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
        {
            keyBuilder.Append($"|{key}-{value}");
        }

        return keyBuilder.ToString();
    }

}