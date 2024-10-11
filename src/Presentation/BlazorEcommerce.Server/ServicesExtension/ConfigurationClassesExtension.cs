using BlazorEcommerce.Application.Models;

namespace BlazorEcommerce.Server.ServicesExtension;
public static class ConfigurationClassesExtension
{
    public static IServiceCollection ConfigureAppSettingData(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailData>(configuration.GetSection("MailSettings"));

        services.Configure<JwtData>(configuration.GetSection("JWT"));

        services.Configure<Urls>(configuration.GetSection("Urls"));

        services.Configure<GoogleData>(configuration.GetSection("Google"));

        services.Configure<GithubData>(configuration.GetSection("Github"));

        services.Configure<HangfireSettingsData>(configuration.GetSection("HangfireSettings"));

        services.Configure<DatabaseConnections>(configuration.GetSection("ConnectionStrings"));

        return services;
    }
}