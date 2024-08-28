using Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repository.Identity;
using Service.ConfigurationData;

namespace API.ServicesExtension;
public static class IdentityConfigurationsExtension
{
	public static IServiceCollection AddIdentityConfigurations(this IServiceCollection services)
	{
		// Identity Context
		var serviceProvider = services.BuildServiceProvider();

		var databaseConnections = serviceProvider.GetRequiredService<IOptions<DatabaseConnections>>().Value;

		services.AddDbContext<IdentityContext>(options =>
		{
			options.UseSqlServer(databaseConnections.IdentityConnection);
		});

		// We need to register three services of identity (UserManager - RoleManager - SignInManager)
		// but we don't need to register all them one by one
		// because we have method (AddIdentity) that will register the three services
		// --- this method has another overload take action to if you need to configure any option of identity
		services.AddIdentity<AppUser, IdentityRole>(option =>
		{
			option.Password.RequireLowercase = true;
			option.Password.RequireUppercase = false;
			option.Password.RequireDigit = false;
			option.Password.RequireNonAlphanumeric = true;
			option.Password.RequiredUniqueChars = 3;
			option.Password.RequiredLength = 6;
		}).AddEntityFrameworkStores<IdentityContext>();
		// ? this because the three services talking to another Store Services
		// such as (UserManager talk to IUserStore to take all services like createAsync)
		// so we allowed dependency injection to this services too

		return services;
	}
}