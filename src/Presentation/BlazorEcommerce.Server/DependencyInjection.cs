namespace BlazorEcommerce.Server;
public static class DependencyInjection
{
    public static WebApplicationBuilder AddDependencies(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        var configuration = builder.Configuration;

        services.ConfigureAppSettingData(configuration);

        var serviceProvider = services.BuildServiceProvider();

        var jwtData = serviceProvider.GetRequiredService<IOptions<JwtData>>().Value;

        var databaseConnections = serviceProvider.GetRequiredService<IOptions<DatabaseConnections>>().Value;

        services.AddControllersWithViews();

        services.AddRazorPages();

		services.AddSwaggerServices();

        services.AddRateLimitingConfigurations();

        services.AddApiVersioningConfigurations();

        services.AddIdentityConfigurations();

        services.AddJwtConfigurations(jwtData);

        services.AddRedis(databaseConnections.RedisConnection);

        services.AddStoreContext(databaseConnections.StoreConnection);

        services.AddHangfireServices(databaseConnections.StoreConnection);

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