using Core.Dtos;
using FluentValidation;

namespace Core.DtosValidators;
public class CodeVerificationValidator: AbstractValidator<CodeVerificationRequest>
{
    public CodeVerificationValidator()
    {
        RuleFor(x => x.VerificationCode)
            .NotEmpty()
            .WithMessage("Verification code is required")
            .Length(6)
            .WithMessage("Verification code must be 6 characters");
    }
}