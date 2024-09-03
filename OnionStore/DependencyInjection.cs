using API.ServicesExtension;
using Microsoft.Extensions.Options;
using Service.ConfigurationData;

namespace API;
public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceProvider = services.BuildServiceProvider();

        var jwtData = serviceProvider.GetRequiredService<IOptions<JWTData>>().Value;

        var databaseConnections = serviceProvider.GetRequiredService<IOptions<DatabaseConnections>>().Value;

        services.AddControllers();

        services.AddSwaggerServices();

        services.AddApiVersioningConfigurations();

        services.ConfigureAppsettingData(configuration);

        services.AddIdentityConfigurations(databaseConnections.IdentityConnection);

        services.AddJWTConfigurations(jwtData);

        services.AddRedis(databaseConnections.RedisConnection);

        services.AddStoreContext(databaseConnections.StoreConnection);

        services.AddHangfireServices(databaseConnections.IdentityConnection);

        services.AddFluentValidation();

        services.AddApplicationServices();

        services.AddCorsPolicy();

        return services;
    }
}