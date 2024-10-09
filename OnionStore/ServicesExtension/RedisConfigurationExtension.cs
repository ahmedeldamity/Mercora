using StackExchange.Redis;

namespace BlazorEcommerce.Server.ServicesExtension;
public static class RedisConfigurationExtension
{
    public static IServiceCollection AddRedis(this IServiceCollection services, string redisConnection)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnection));

        return services;
    }

}