using BlazorEcommerce.Application.Dtos;
using FluentValidation;

namespace BlazorEcommerce.Application.Validations;
public class LoginValidator: AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid")
            .MaximumLength(50)
            .WithMessage("Email must not exceed 50 characters.");
    }
}