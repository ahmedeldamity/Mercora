using StackExchange.Redis;

namespace API.ServicesExtension;
public static class RedisConfigurationExtension
{
    public static IServiceCollection AddRedis(this IServiceCollection services, string redisConnection)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(redisConnection);
        });

        return services;
    }

}