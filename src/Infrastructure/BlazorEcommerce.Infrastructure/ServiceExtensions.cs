namespace BlazorEcommerce.Infrastructure;
public static class ServiceExtensions
{
	public static void ConfigureApplication(this IServiceCollection services)
	{
		services.AddAutoMapper(Assembly.GetExecutingAssembly());
		services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
	}
}