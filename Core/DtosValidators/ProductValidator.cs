using Core.Dtos;
using FluentValidation;

namespace Core.DtosValidators;
public class ProductValidator: AbstractValidator<ProductRequest>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(100)
            .WithMessage("Description must not exceed 100 characters");

        RuleFor(x => x.ImageCover)
            .NotEmpty()
            .WithMessage("Image cover is required")
            .MaximumLength(100)
            .WithMessage("Image cover must not exceed 100 characters");

        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("Images are required");

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required")
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.RatingsAverage)
            .NotEmpty()
            .WithMessage("Ratings average is required");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price is required")
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.BrandId)
            .NotEmpty()
            .WithMessage("Brand id is required")
            .GreaterThan(0)
            .WithMessage("Brand id must be greater than 0");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category id is required")
            .GreaterThan(0)
            .WithMessage("Category id must be greater than 0");
    }

}