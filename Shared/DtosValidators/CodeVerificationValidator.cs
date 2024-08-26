using FluentValidation;
using Shared.Dtos;

namespace Shared.DtosValidators;
public class CodeVerificationValidator: AbstractValidator<CodeVerificationRequest>
{
    public CodeVerificationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");

        RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .WithMessage("Verification code is required")
            .Length(6)
            .WithMessage("Verification code must be 6 characters");
    }
}