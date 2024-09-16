using Core.Dtos;
using FluentValidation;

namespace API.DtosValidators;
public class ProductCategoryValidator: AbstractValidator<ProductCategoryRequest>
{
    public ProductCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");
    }
}