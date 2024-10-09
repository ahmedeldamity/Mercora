using BlazorEcommerce.Application.Dtos;
using FluentValidation;

namespace BlazorEcommerce.Application.Validations;
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