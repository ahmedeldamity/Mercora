using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace BlazorEcommerce.Application;
public static class ServiceExtensions
{
	public static void ConfigureApplication(this IServiceCollection services)
	{
		services.AddAutoMapper(Assembly.GetExecutingAssembly());
		services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
	}
}