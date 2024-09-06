using Core.Dtos;
using FluentValidation;

namespace API.DtosValidators;
public class OrderItemValidator: AbstractValidator<OrderItemRequest>
{
    public OrderItemValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Id must be valid");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("ProductId must be valid");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MaximumLength(50)
            .WithMessage("Product name must not exceed 50 characters");

        RuleFor(x => x.ImageCover)
            .NotEmpty()
            .WithMessage("Image cover is required")
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("Image cover must be a valid URL")
            .MaximumLength(50)
            .WithMessage("Image cover must not exceed 50 characters");

        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Price must be valid");

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Quantity must be valid");
    }
}