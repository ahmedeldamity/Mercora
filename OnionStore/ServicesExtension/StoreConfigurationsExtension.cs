using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repository.Store;
using Shared.ConfigurationData;

namespace API.ServicesExtension;
public static class StoreConfigurationsExtension
{
    public static IServiceCollection AddStoreContext(this IServiceCollection services)
    {
        // Register Store Context
        var serviceProvider = services.BuildServiceProvider();
        var databaseConnections = serviceProvider.GetRequiredService<IOptions<DatabaseConnections>>().Value;
        services.AddDbContext<StoreContext>(options =>
        {
            options.UseSqlServer(databaseConnections.StoreConnection);
        });

        return services;
    }
}