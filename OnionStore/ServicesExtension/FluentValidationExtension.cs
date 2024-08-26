using FluentValidation.AspNetCore;
using FluentValidation;
using Shared.DtosValidators;

namespace API.ServicesExtension
{
    public static class FluentValidationExtension
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterRequestDtoValidator>();

            return services;
        }
    }
}
