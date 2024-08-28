using Core.Dtos;
using FluentValidation;

namespace Core.DtosValidators;
public class BasketItemValidator : AbstractValidator<BasketItemRequest>
{
    public BasketItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(50)
            .WithMessage("Name must not exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price is required")
            .GreaterThan(0)
            .WithMessage("Price must be valid");

        RuleFor(x => x.ImageCover)
            .NotEmpty()
            .WithMessage("Image cover is required")
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("Image cover must be a valid URL")
            .MaximumLength(50)
            .WithMessage("Image cover must not exceed 50 characters");

        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("Images are required");

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required")
            .GreaterThan(0)
            .WithMessage("Quantity must be Valid");

        RuleFor(x => x.RatingsAverage)
            .NotEmpty()
            .WithMessage("Ratings average is required");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Brand is required");
    }
}