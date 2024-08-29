using Core.DtosValidators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace API.ServicesExtension;
public static class FluentValidationExtension
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<RegisterValidator>()
            .AddValidatorsFromAssemblyContaining<LoginValidator>()
            .AddValidatorsFromAssemblyContaining<CodeVerificationValidator>()
            .AddValidatorsFromAssemblyContaining<ChangePasswordValidator>()
            .AddValidatorsFromAssemblyContaining<BasketItemValidator>()
            .AddValidatorsFromAssemblyContaining<OrderAddressValidator>()
            .AddValidatorsFromAssemblyContaining<OrderItemValidator>()
            .AddValidatorsFromAssemblyContaining<OrderValidator>()
            .AddValidatorsFromAssemblyContaining<UserAddressValidator>()
            .AddValidatorsFromAssemblyContaining<EmailValidator>();

        return services;
    }
}