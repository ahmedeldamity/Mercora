namespace BlazorEcommerce.Application.Validations;
public class OrderValidator: AbstractValidator<OrderRequest>
{
    public OrderValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty()
            .WithMessage("CartId is required")
            .MaximumLength(128)
            .WithMessage("CartId must not exceed 128 characters");

        RuleFor(x => x.ShippingAddress)
            .NotNull()
            .WithMessage("ShippingAddress is required");
    }
}