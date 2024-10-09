using BlazorEcommerce.Application.Models;
using BlazorEcommerce.Server.Extensions;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.Extensions.Options;

namespace BlazorEcommerce.Server.ServicesExtension;
public static class HangfireConfigurationsExtension
{
	public static IServiceCollection AddHangfireServices(this IServiceCollection services, string storeConnection)
	{
        // Add Hangfire Services
        services.AddHangfire(x => x.UseSqlServerStorage(storeConnection));

        // Register Hangfire Services
        services.AddHangfireServer();

        return services;
	}

    public static WebApplication UseHangfireDashboardAndRecurringJob(this WebApplication app, IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();

        var hangfireData = serviceProvider.GetRequiredService<IOptions<HangfireSettingsData>>().Value;

        app.UseHangfireDashboard(hangfireData.DashboardUrl, new DashboardOptions()
        {
            Authorization =
            [
                new HangfireCustomBasicAuthenticationFilter
                {
                    User  = hangfireData.UserName,
                    Pass  = hangfireData.Password
                }
            ],
            DashboardTitle = hangfireData.ServerName,

        });

        RecurringJob.AddOrUpdate<DataDeletionJob>("data-deletion-job", x => x.Execute(), Cron.Monthly(1));

        return app;
    }

}