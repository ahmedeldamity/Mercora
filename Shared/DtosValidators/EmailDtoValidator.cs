using FluentValidation;
using Shared.Dtos;

namespace Shared.DtosValidators;
public class EmailDtoValidator: AbstractValidator<EmailDto>
{
    public EmailDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");
    }
}