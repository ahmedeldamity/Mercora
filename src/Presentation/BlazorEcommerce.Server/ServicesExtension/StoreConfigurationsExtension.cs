﻿namespace BlazorEcommerce.Server.ServicesExtension;
public static class StoreConfigurationsExtension
{
    public static IServiceCollection AddStoreContext(this IServiceCollection services, string storeConnection)
    {
        services.AddDbContext<StoreContext>(options =>
        {
            options.UseSqlServer(storeConnection);
        });

        return services;
    }

}