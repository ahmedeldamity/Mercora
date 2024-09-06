using Core.Dtos;
using FluentValidation;

namespace Core.DtosValidators;
public class TokenIdValidator: AbstractValidator<TokenIdRequest>
{
    public TokenIdValidator()
    {
        RuleFor(x => x.TokenId)
            .NotEmpty()
            .WithMessage("TokenId is required")
            .MaximumLength(300)
            .WithMessage("TokenId must not exceed 100 characters");
    }
}