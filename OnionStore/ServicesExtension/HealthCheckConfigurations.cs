using API.Health;
using Service.ConfigurationData;

namespace API.ServicesExtension
{
    public static class HealthCheckConfigurations
    {
        public static IServiceCollection AddHealthCheckConfigurations(this IServiceCollection services, DatabaseConnections connections)
        {
            services.AddHealthChecks()
                .AddSqlServer(connections.IdentityConnection, name: "IdentityDb-check")
                .AddSqlServer(connections.StoreConnection, name: "StoreDb-check")
                .AddRedis(connections.RedisConnection, name: "Redis-check")
                .AddHangfire(t => t.MinimumAvailableServers = 1, name: "Hangfire-check")
                .AddCheck<MailHealthCheck>(name: "MailService-check");

            return services;
        }
    }
}