using Microsoft.Extensions.Options;
using Shared.ConfigurationData;
using StackExchange.Redis;

namespace API.ServicesExtension;
public static class RedisConfigurationExtension
{
    public static IServiceCollection AddRedis(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var databaseConnections = serviceProvider.GetRequiredService<IOptions<DatabaseConnections>>().Value;

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(databaseConnections.RedisConnection);
        });

        return services;
    }
}