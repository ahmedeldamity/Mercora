using FluentValidation;
using Shared.Dtos;

namespace Shared.DtosValidators
{
    public class OrderDtoValidator: AbstractValidator<OrderDto>
    {
        public OrderDtoValidator()
        {
            RuleFor(x => x.BasketId)
                .NotEmpty()
                .WithMessage("BasketId is required")
                .MaximumLength(128)
                .WithMessage("BasketId must not exceed 128 characters");

            RuleFor(x => x.DeliveryMethodId)
                .NotEmpty()
                .WithMessage("DeliveryMethodId is required")
                .GreaterThan(0)
                .WithMessage("DeliveryMethodId must be greater than 0");

            RuleFor(x => x.ShippingAddress)
                .NotNull()
                .WithMessage("ShippingAddress is required");
        }
    }
}