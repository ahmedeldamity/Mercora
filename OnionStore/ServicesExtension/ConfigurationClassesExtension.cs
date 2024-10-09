using BlazorEcommerce.Application.Models;

namespace BlazorEcommerce.Server.ServicesExtension;
public static class ConfigurationClassesExtension
{
    public static IServiceCollection ConfigureAppSettingData(this IServiceCollection services, IConfiguration configuration)
    {
        // Take email setting data form appsetting to MailSettings class
        services.Configure<MailData>(configuration.GetSection("MailSettings"));

        // Take JWT setting data form appsetting to JWTData class
        services.Configure<JwtData>(configuration.GetSection("JWT"));

        // Take HangFire setting data form appsetting to HangfireSettingsData class
        services.Configure<HangfireSettingsData>(configuration.GetSection("HangfireSettings"));

        // Take Database Connections data form appsetting to DatabaseConnections class
        services.Configure<DatabaseConnections>(configuration.GetSection("ConnectionStrings"));

        return services;
    }
}