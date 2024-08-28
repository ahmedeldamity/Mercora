using Core.Dtos;
using FluentValidation;

namespace Core.DtosValidators;
public class OrderValidator: AbstractValidator<OrderRequest>
{
    public OrderValidator()
    {
        RuleFor(x => x.BasketId)
            .NotEmpty()
            .WithMessage("BasketId is required")
            .MaximumLength(128)
            .WithMessage("BasketId must not exceed 128 characters");

        RuleFor(x => x.ShippingAddress)
            .NotNull()
            .WithMessage("ShippingAddress is required");
    }
}