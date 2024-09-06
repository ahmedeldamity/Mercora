using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace API.ServicesExtension;
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