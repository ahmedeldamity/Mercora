using BlazorEcommerce.Application.Dtos;
using FluentValidation;

namespace BlazorEcommerce.Application.Validations;
public class CodeVerificationValidator: AbstractValidator<RegisterCodeVerificationRequest>
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