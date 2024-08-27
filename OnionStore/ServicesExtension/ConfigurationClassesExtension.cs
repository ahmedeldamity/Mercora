using Shared.ConfigurationData;

namespace API.ServicesExtension;
public static class ConfigurationClassesExtension
{
    public static IServiceCollection ConfigureAppsettingData(this IServiceCollection services, IConfiguration configuration)
    {
        // Take email setting data form appsetting to MailSettings class
        services.Configure<MailData>(configuration.GetSection("MailSettings"));

        // Take JWT setting data form appsetting to JWTData class
        services.Configure<JWTData>(configuration.GetSection("JWT"));

        // Take HangFire setting data form appsetting to HangfireSettingsData class
        services.Configure<HangfireSettingsData>(configuration.GetSection("HangfireSettings"));

        // Take Database Connections data form appsetting to DatabaseConnections class
        services.Configure<DatabaseConnections>(configuration.GetSection("ConnectionStrings"));

        return services;
    }
}