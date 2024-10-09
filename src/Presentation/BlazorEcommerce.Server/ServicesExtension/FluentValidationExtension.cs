using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace BlazorEcommerce.Server.ServicesExtension;
public static class FluentValidationExtension
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}