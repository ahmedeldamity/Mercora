using API.ServicesExtension;
using Microsoft.Extensions.Options;
using Serilog;
using Service.ConfigurationData;

namespace API;
public static class DependencyInjection
{
    public static WebApplicationBuilder AddDependencies(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        var configuration = builder.Configuration;

        services.ConfigureAppsettingData(configuration);

        var serviceProvider = services.BuildServiceProvider();

        var jwtData = serviceProvider.GetRequiredService<IOptions<JWTData>>().Value;

        var databaseConnections = serviceProvider.GetRequiredService<IOptions<DatabaseConnections>>().Value;

        services.AddControllers();

        services.AddSwaggerServices();

        services.AddApiVersioningConfigurations();

        services.AddIdentityConfigurations(databaseConnections.IdentityConnection);

        services.AddJWTConfigurations(jwtData);

        services.AddRedis(databaseConnections.RedisConnection);

        services.AddStoreContext(databaseConnections.StoreConnection);

        services.AddHangfireServices(databaseConnections.IdentityConnection);

        services.AddFluentValidation();

        services.AddApplicationServices();

        services.AddHealthCheckConfigurations(databaseConnections);

        services.AddCorsPolicy();

        builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration);
        });

        return builder;
    }
}