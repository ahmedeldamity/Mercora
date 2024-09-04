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

        //services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);

        //services.AddOutputCache(options =>
        //{
        //    options.AddBasePolicy(x => x.Expire(TimeSpan.FromSeconds(60)));

        //    options.AddPolicy("MyCustom", x => x.Expire(TimeSpan.FromSeconds(10)));
        //});

        return services;
    }

}