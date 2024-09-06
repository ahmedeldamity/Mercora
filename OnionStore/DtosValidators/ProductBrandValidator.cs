using Core.Dtos;
using FluentValidation;

namespace API.DtosValidators;

public class ProductBrandValidator: AbstractValidator<ProductBrandRequest>
{
    public ProductBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.ImageCover)
            .NotEmpty()
            .WithMessage("Image cover is required")
            .MaximumLength(100)
            .WithMessage("Image cover must not exceed 100 characters");
    }

}