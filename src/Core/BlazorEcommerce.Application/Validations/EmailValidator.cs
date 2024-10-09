using BlazorEcommerce.Application.Dtos;
using FluentValidation;

namespace BlazorEcommerce.Application.Validations;
public class EmailValidator : AbstractValidator<EmailRequest>
{
    public EmailValidator()
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