using FluentValidation;
using Shared.Dtos;

namespace Shared.DtosValidators;
public class EmailValidator: AbstractValidator<EmailRequest>
{
    public EmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");
    }
}