using Microsoft.EntityFrameworkCore;
using Repository.Store;

namespace API.ServicesExtension
{
    public static class StoreConfigurationsExtension
    {
        public static IServiceCollection AddStoreContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Store Context
            services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("StoreConnection"));
            });

            return services;
        }
    }
}
