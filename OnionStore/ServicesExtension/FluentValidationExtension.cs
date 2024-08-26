using FluentValidation.AspNetCore;
using FluentValidation;
using Shared.DtosValidators;

namespace API.ServicesExtension;
public static class FluentValidationExtension
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<RegisterValidator>()
            .AddValidatorsFromAssemblyContaining<LoginValidator>()
            .AddValidatorsFromAssemblyContaining<EmailValidator>()
            .AddValidatorsFromAssemblyContaining<CodeVerificationValidator>()
            .AddValidatorsFromAssemblyContaining<ChangePasswordValidator>()
            .AddValidatorsFromAssemblyContaining<BasketItemValidator>()
            .AddValidatorsFromAssemblyContaining<OrderAddressValidator>()
            .AddValidatorsFromAssemblyContaining<OrderItemValidator>()
            .AddValidatorsFromAssemblyContaining<OrderValidator>()
            .AddValidatorsFromAssemblyContaining<UserAddressValidator>();

        return services;
    }
}