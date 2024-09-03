using API.Extensions;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.Extensions.Options;
using Service.ConfigurationData;

namespace API.ServicesExtension;
public static class HangfireConfigurationsExtension
{
	public static IServiceCollection AddHangfireServices(this IServiceCollection services, string identityConnection)
	{
        // Add Hangfire Services
        services.AddHangfire(x => x.UseSqlServerStorage(identityConnection));

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